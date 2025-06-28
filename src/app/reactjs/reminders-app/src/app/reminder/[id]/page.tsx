import EditClient from './edit-client';

// Generate static params for static export
export async function generateStaticParams() {
  // For static export, we'll generate a few example IDs
  // In a real scenario, you might fetch this from your API
  return [
    { id: '1' },
    { id: '2' },
    { id: '3' },
    { id: '4' },
    { id: '5' },
  ];
}

export default function Edit() {
  return <EditClient />;
}
