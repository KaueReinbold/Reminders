import { Metadata } from 'next';
import EditClient from './edit-client';

export const metadata: Metadata = {
  title: 'Edit Reminder',
};

export function generateStaticParams() {
  return [];
}

export default async function EditPage({ params }: { params: Promise<{ id: string }> }) {
  await params;
  return <EditClient />;
}
