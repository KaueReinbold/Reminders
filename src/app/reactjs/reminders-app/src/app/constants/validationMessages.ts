export const validationMessages = {
  // Matches server-side messages from ReminderViewModel
  required: "The field is Required",
  maxLength: "The field {field} must be a text with a maximum length of '{maxLength}'.",
  invalidDate: "Invalid {field}"
};

type ReplaceValuesInput = Record<string | number, string | number> | Array<string | number>;

/**
 * Replace placeholders in a validation message with actual values.
 * Supports named placeholders like `{field}` and indexed placeholders like `{0}`.
 * @param message - The validation message template.
 * @param values - An object with keys matching placeholder names, or an array for indexed placeholders.
 */
export function replaceValues(message: string, values: ReplaceValuesInput): string {
  if (Array.isArray(values)) {
    return values.reduce<string>((msg, val, idx) => msg.split(`{${idx}}`).join(String(val)), message);
  }

  return Object.keys(values).reduce((msg, key) => {
    const placeholder = `{${key}}`;
    return msg.split(placeholder).join(String((values as Record<string, string | number>)[key]));
  }, message);
}
