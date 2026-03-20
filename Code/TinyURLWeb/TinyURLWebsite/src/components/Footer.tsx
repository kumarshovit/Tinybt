import React from 'react'

const Footer = () => {
  return (
    <div>
         <footer className="bg-gray-900 text-gray-300 py-10">

        <div className="max-w-7xl mx-auto px-6 grid md:grid-cols-4 gap-8">

          <div>
            <h3 className="text-white font-bold mb-3">
              TinyURL
            </h3>
            <p>
              Modern link management platform
              built for developers and marketers.
            </p>
          </div>

          <div>
            <h4 className="text-white mb-3">Product</h4>
            <p>Features</p>
            <p>Analytics</p>
            <p>API</p>
          </div>

          <div>
            <h4 className="text-white mb-3">Company</h4>
            <p>About</p>
            <p>Blog</p>
            <p>Careers</p>
          </div>

          <div>
            <h4 className="text-white mb-3">Legal</h4>
            <p>Privacy</p>
            <p>Terms</p>
          </div>

        </div>

      </footer>

    </div>
  )
}

export default Footer