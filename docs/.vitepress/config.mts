import {defineConfig} from 'vitepress'

// https://vitepress.dev/reference/site-config
export default defineConfig({
  title: "Raiqub Expressions",
  lang: "en-US",
  description: "A library that provides abstractions for creating specifications and query strategies using LINQ expressions",
  head: [
    ['link', { rel: "apple-touch-icon", sizes: "180x180", href: "/Expressions/apple-touch-icon.png" }],
    ['link', { rel: "icon", type: "image/png", sizes: "32x32", href: "/Expressions/favicon-32x32.png" }],
    ['link', { rel: "icon", type: "image/png", sizes: "16x16", href: "/Expressions/favicon-16x16.png" }],
    ['link', { rel: "manifest", href: "/Expressions/manifest.json" }],
    ['meta', { property: 'og:title', content: 'Raiqub Expressions' }],
    ['meta', { property: 'og:type', content: 'website' }],
    ['meta', {
      property: 'og:description',
      content: 'A library that provides abstractions for creating specifications and query strategies using LINQ expressions'
    }],
    ['meta', { property: 'og:url', content: 'https://fgodoy.me/Expressions/' }]
  ],

  lastUpdated: true,

  themeConfig: {
    // https://vitepress.dev/reference/default-theme-config
    nav: [
      { text: 'Home', link: '/' },
      { text: 'Intro', link: '/introduction' },
      { text: 'Migration', link: '/migration-guide' },
      { text: 'Discussions', link: 'https://github.com/skarllot/Expressions/discussions' }
    ],

    logo: '/favicon-128x128.png',

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
            { text: 'Introduction', link: '/specification/' },
            { text: 'Creating Specifications', link: '/specification/creating-specifications' },
            { text: 'Usage Patterns', link: '/specification/usage-patterns' },
            { text: 'Testing', link: '/specification/testing' },
            { text: 'Best Practices', link: '/specification/best-practices' }
          ]
        },
        {
          text: 'Query Strategy',
          items: [
            { text: 'Introduction', link: '/query-strategy/' },
            { text: 'Creating Query Strategies', link: '/query-strategy/creating-strategies' },
            { text: 'Common Patterns', link: '/query-strategy/common-patterns' },
            { text: 'Testing', link: '/query-strategy/testing' },
            { text: 'Best Practices', link: '/query-strategy/best-practices' }
          ]
        },
        {
          text: 'Database Session',
          items: [
            { text: 'Managing Database Sessions', link: '/database-session/' },
            { text: 'Bounded Contexts', link: '/database-session/bounded-contexts' }
          ]
        },
        {
          text: 'Entity Framework',
          items: [
            { text: 'Using EF Core', link: '/ef-core/' },
            { text: 'Bounded Contexts', link: '/ef-core/bounded-contexts' },
            { text: 'Custom SQL Query', link: '/ef-core/custom-sql' },
            { text: 'Split Query', link: '/ef-core/split-query' }
          ]
        },
        {
          text: 'Marten',
          items: [
            { text: 'Using Marten', link: '/marten/' },
            { text: 'Bounded Contexts', link: '/marten/bounded-contexts' }
          ]
        },
        {
          text: 'Best Practices',
          items: [
            { text: 'Testing Strategies', link: 'best-practices/testing' },
            { text: 'Performance Optimization', link: 'best-practices/performance' }
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
  base: '/Expressions/',
  sitemap: {
    hostname: "https://fgodoy.me/Expressions/"
  }
})
