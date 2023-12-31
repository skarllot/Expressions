import { defineConfig } from 'vitepress'

// https://vitepress.dev/reference/site-config
export default defineConfig({
  title: "Raiqub Expressions",
  lang: "en-US",
  description: "A library that provides abstractions for creating specifications and query strategies using LINQ expressions",
  themeConfig: {
    // https://vitepress.dev/reference/default-theme-config
    nav: [
      { text: 'Home', link: '/' },
      { text: 'Intro', link: '/introduction' },
      { text: 'Migration', link: '/migration-guide' }
    ],

    logo: '/logo-red-white-favicon-128.png',

    sidebar: {
      '/': [
        {
          text: 'Introduction',
          items: [
            { text: 'What is Raiqub Expressions', link: '/introduction' },
            { text: 'Getting Started', link: '/getting-started' }
          ]
        },
        {
          text: 'Specification',
          items: [
            { text: 'Building Specifications', link: '/specification/' }
          ]
        },
        {
          text: 'Query Strategy',
          items: [
            { text: 'Building Query Strategies', link: '/query-strategy/' }
          ]
        },
        {
          text: 'Database Session',
          items: [
            { text: 'Managing Database Sessions', link: '/database-session/' }
          ]
        },
        {
          text: 'Entity Framework',
          items: [
            { text: 'Using EF Core', link: '/ef-core/' },
            { text: 'Custom SQL Query', link: '/ef-core/custom-sql' },
            { text: 'Split Query', link: '/ef-core/split-query' }
          ]
        },
        {
          text: 'Marten',
          items: [
            { text: 'Using Marten', link: '/marten/' }
          ]
        }
      ]
    },

    socialLinks: [
      { icon: 'github', link: 'https://github.com/skarllot/Expressions' }
    ],

    footer: {
      message: 'Released under the MIT License.',
      copyright: 'Copyright © Fabricio Godoy and contributors.'
    },
    editLink: {
      pattern: 'https://github.com/skarllot/Expressions/edit/main/docs/:path',
      text: 'Edit this page on GitHub'
    }
  },
  head: [
    ['link', { rel: "icon", type: "image/png", sizes: "32x32", href: "/logo-red-white-favicon-32.png" }],
    ['link', { rel: "icon", type: "image/png", sizes: "16x16", href: "/logo-red-white-favicon-16.png" }],
    ['meta', { property: 'og:title', content: 'Raiqub Expressions' }],
    ['meta', { property: 'og:type', content: 'website' }],
    ['meta', { property: 'og:description', content: 'A library that provides abstractions for creating specifications and query strategies using LINQ expressions' }]
  ],
  base: '/Expressions/',
  sitemap: {
    hostname: "https://fgodoy.me/Expressions/"
  }
})
