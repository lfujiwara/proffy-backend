import nodemailer from 'nodemailer';
import constants from './constants';

const transporter = nodemailer.createTransport({
  host: constants.SMTP_HOST,
  port: Number(constants.SMTP_PORT),
  tls: {
    ciphers: 'SSLv3',
  },
  auth: {
    user: constants.SMTP_USER,
    pass: constants.SMTP_PASS,
  },
});

export default transporter;
