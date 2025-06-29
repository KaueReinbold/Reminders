import EditClient from './edit-client';

// For static export - dynamic routes require generateStaticParams
export async function generateStaticParams() {
  // Return empty array for client-side routing
  return [];
}

export default function EditPage() {
  return <EditClient />;
}