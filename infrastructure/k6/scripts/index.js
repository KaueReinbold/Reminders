const isNumeric = (value) => /^\d+$/.test(value);

const default_vus = 5;

const target_vus_env = `${__ENV.TARGET_VUS}`;
const target_vus = isNumeric(target_vus_env) ? Number(target_vus_env) : default_vus;
export const baseUrl = `${__ENV.BASE_URL}`;

export let options = {
  stages: [
      // Ramp-up from 1 to TARGET_VUS virtual users (VUs) in 5s
      { duration: "5s", target: target_vus },

      // Stay at rest on TARGET_VUS VUs for 10s
      { duration: "10s", target: target_vus },

      // Ramp-down from TARGET_VUS to 0 VUs for 5s
      { duration: "5s", target: 0 }
  ]
};
