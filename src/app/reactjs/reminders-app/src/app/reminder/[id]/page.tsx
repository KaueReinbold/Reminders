import { Metadata } from 'next';
import EditClient from './edit-client';

export const metadata: Metadata = {
  title: 'Edit Reminder',
};

export const dynamicParams = false;

export function generateStaticParams() {
  // Next.js 15 requires at least one path for dynamic routes when using `output: 'export'`.
  // Real IDs are loaded client-side; unknown paths are handled via the SPA 404 fallback.
  return [{ id: 'placeholder' }];
}

export default async function EditPage({ params }: { params: Promise<{ id: string }> }) {
  await params;
  return <EditClient />;
}
