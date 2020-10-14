import axios from 'axios';
import cors from 'cors';
import Express from 'express';
import Email from 'email-templates';
import jwt from 'jsonwebtoken';
import path from 'path';
import constants from './constants';
import { registerSchema, IRegisterRequest, confirmSchema } from './schemas';
import transporter from './mailer';

const server = Express();
server.use(Express.json());
server.use(
  cors({
    origin: constants.PROFFY_ORIGIN,
  })
);

const email = new Email({
  message: {},
  juice: true,
  juiceResources: {
    preserveImportant: true,
    webResources: {
      relativeTo: path.resolve('templates'),
    },
  },
  views: {
    options: {
      extension: 'ejs',
    },
  },
});

server.post('/register', async (req, res) => {
  await registerSchema
    .validateAsync(req.body)
    .then(async (value: IRegisterRequest) => {
      const userDataToken = jwt.sign(req.body, constants.JWT_SECRET, {
        expiresIn: '10m',
      });
      await transporter.sendMail({
        to: value.email,
        from: constants.SMTP_USER,
        subject: 'Cadastro - Proffy',
        html: await email.render(`${__dirname}/templates/confirm-email.ejs`, {
          name: value.firstName,
          url: `${constants.PROFFY_REGISTRATION_URL}?token=${userDataToken}`,
        }),
      });
    })
    .then(() => res.sendStatus(201))
    .catch((err) => res.status(400).json(err));
});

server.post('/confirm', async (req, res) => {
  let userData;
  try {
    const token: string = req.body.token;
    userData = jwt.decode(token);
    if (userData.exp && new Date() > new Date(Number(userData.exp) * 1000))
      throw new Error('Token expired');
    delete userData.exp;
    delete userData.iat;
  } catch (err) {
    res.status(400).json(err);
    return;
  }

  const password = req.body.password;

  await confirmSchema
    .validateAsync({ ...userData, password })
    .catch((err) => res.status(400).json(err))
    .then((data) =>
      axios.post(`${constants.PROFFY_BACKEND}/users`, data, {
        headers: { 'X-API-KEY': constants.PROFFY_API_KEY },
      })
    )
    .then(() => {
      res.sendStatus(201);
    })
    .catch((err) => res.status(500).json(err));
});

export default server;
