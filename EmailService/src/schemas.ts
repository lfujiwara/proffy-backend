import Joi from 'joi';

export interface IRegisterRequest {
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
}

export interface IConfirmRequest {}

export const registerSchema = Joi.object({
  email: Joi.string().email().min(6).required(),
  firstName: Joi.string().alphanum().min(2).required(),
  lastName: Joi.string().alphanum().min(2).required(),
  phoneNumber: Joi.string()
    .max(15)
    .pattern(/^[0-9+]+$/),
});
