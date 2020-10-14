import Joi from 'joi';

export interface IRegisterRequest {
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
}

export interface IConfirmRequest extends IRegisterRequest {
  password: string;
}

const registerInternalSchema = {
  email: Joi.string().email().min(6).required(),
  firstName: Joi.string().min(2).required(),
  lastName: Joi.string().min(2).required(),
  phoneNumber: Joi.string()
    .max(15)
    .pattern(/^[0-9+]+$/)
    .required(),
};

export const registerSchema = Joi.object(registerInternalSchema);

export const confirmSchema = Joi.object({
  ...registerInternalSchema,
  password: Joi.string().min(10),
});
