import { Metadata } from 'next';
import EditClient from './edit-client';

export const metadata: Metadata = {
  title: 'Edit Reminder',
};

// Required for Next.js static export with dynamic routes  
export async function generateStaticParams(): Promise<{ id: string }[]> {
  // Return empty array - dynamic routes will be handled client-side
  // via the 404.html redirect mechanism
  return [];
}

export default async function EditPage({ params }: { params: Promise<{ id: string }> }) {
  // In Next.js 15, params is a Promise
  await params;
  return <EditClient />;
}
