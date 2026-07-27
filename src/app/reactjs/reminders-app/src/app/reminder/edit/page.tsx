import { Metadata } from 'next';
import EditClient from './edit-client';

export const metadata: Metadata = {
  title: 'Edit Reminder',
};

// Static route: the reminder id comes from the `id` query parameter so the
// page works both in the server build (docker) and the static export (Pages).
export default function EditPage() {
  return <EditClient />;
}
