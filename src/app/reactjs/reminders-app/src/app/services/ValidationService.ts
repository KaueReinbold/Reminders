import { validationMessages, replaceValues } from '@/app/constants/validationMessages';

export default class ValidationService {
  static validateTitle(title?: string | null): string | null {
    const fieldName = 'Title';

    if (!title || title.trim() === '') {
      return validationMessages.required; // server message "The field is Required"
    }

    if (title.length > 50) {
      return replaceValues(validationMessages.maxLength, { field: fieldName, maxLength: 50 });
    }

    return null;
  }

  static validateDescription(description?: string | null): string | null {
    const fieldName = 'Description';

    if (!description || description.trim() === '') {
      return validationMessages.required;
    }

    if (description.length > 200) {
      return replaceValues(validationMessages.maxLength, { field: fieldName, maxLength: 200 });
    }

    return null;
  }

  static validateLimitDate(limitDate?: string | null): string | null {
    const fieldName = 'Limit Date';

    // LimitDate is optional on the client; only validate format when provided
    if (!limitDate || String(limitDate).trim() === '') {
      return null;
    }

    const parsed = Date.parse(limitDate);
    if (Number.isNaN(parsed)) {
      return replaceValues(validationMessages.invalidDate, { field: fieldName });
    }

    return null;
  }
}
