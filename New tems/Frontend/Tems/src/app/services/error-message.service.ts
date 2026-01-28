import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ErrorMessageService {
  
  private errorMessages: { [key: number]: string } = {
    400: 'Bad request. Please check your input.',
    401: 'Unauthorized. Please login again.',
    403: 'You do not have permission to perform this action.',
    404: 'Resource not found.',
    409: 'Asset with this TEMS ID or Serial Number already exists.',
    422: 'Validation failed. Please check your input.',
    500: 'Internal server error. Please try again later.',
    503: 'Service temporarily unavailable. Please try again later.'
  };

  getErrorMessage(statusCode: number, defaultMessage: string = 'An error occurred'): string {
    return this.errorMessages[statusCode] || defaultMessage;
  }

  setErrorMessage(statusCode: number, message: string): void {
    this.errorMessages[statusCode] = message;
  }
}
