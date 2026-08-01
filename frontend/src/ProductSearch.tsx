import { useState } from 'react'

interface Product {
  id: number
  name: string
  description: string
}

function ProductSearch() {
  const [query, setQuery] = useState('')
  const [submittedQuery, setSubmittedQuery] = useState('')
  const [results, setResults] = useState<Product[]>([])

  const handleSearch = async () => {
    setSubmittedQuery(query)
    const response = await fetch(
      `http://localhost:5001/api/products/search?query=${encodeURIComponent(query)}`,
    )
    const data = await response.json()
    setResults(data)
  }

  return (
    <div>
      <h2>Product Search</h2>
      <input value={query} onChange={(e) => setQuery(e.target.value)} />
      <button onClick={handleSearch}>Search</button>

      {/* VULNERABLE: reflected XSS (A05:2025 - Injection) - renders raw user input as HTML */}
      <p
        dangerouslySetInnerHTML={{
          __html: `You searched for: ${submittedQuery}`,
        }}
      />

      <ul>
        {results.map((product) => (
          <li key={product.id}>
            <strong>{product.name}</strong>: {product.description}
          </li>
        ))}
      </ul>
    </div>
  )
}

export default ProductSearch
