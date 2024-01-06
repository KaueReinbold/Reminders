"use client";

import React, { useState } from "react";
import { QueryClient, QueryClientProvider } from "react-query";

function QueryProviders({ children }: React.PropsWithChildren) {
    const [client] = useState(new QueryClient());

    return (
        <QueryClientProvider client={client}>
            {children}
        </QueryClientProvider>
    );
}

export default QueryProviders;
