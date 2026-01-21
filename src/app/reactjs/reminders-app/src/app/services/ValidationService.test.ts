import ValidationService from '@/app/services/ValidationService';

describe('ValidationService', () => {
  describe('validateTitle', () => {
    it('returns required message when empty', () => {
      expect(ValidationService.validateTitle('')).toBe('The field is Required');
    });

    it('returns max length message when too long', () => {
      const long = 'a'.repeat(51);
      expect(ValidationService.validateTitle(long)).toBe("The field Title must be a text with a maximum length of '50'.");
    });

    it('returns null for valid title', () => {
      expect(ValidationService.validateTitle('Hello')).toBeNull();
    });
  });

  describe('validateDescription', () => {
    it('returns required message when empty', () => {
      expect(ValidationService.validateDescription('')).toBe('The field is Required');
    });

    it('returns max length message when too long', () => {
      const long = 'a'.repeat(201);
      expect(ValidationService.validateDescription(long)).toBe("The field Description must be a text with a maximum length of '200'.");
    });

    it('returns null for valid description', () => {
      expect(ValidationService.validateDescription('Some desc')).toBeNull();
    });
  });

  describe('validateLimitDate', () => {
    it('returns null when empty (optional)', () => {
      expect(ValidationService.validateLimitDate('')).toBeNull();
    });

    it('returns invalid message for bad date', () => {
      expect(ValidationService.validateLimitDate('not-a-date')).toBe('Invalid Limit Date');
    });

    it('returns null for valid date', () => {
      expect(ValidationService.validateLimitDate('2024-01-01T00:00:00Z')).toBeNull();
    });
  });
});
