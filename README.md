# URL Shortner

In this repository we will be building a URL shortner without the use of an AI agent to write code. The whole purpose of it is to get back in touch with writing code and thinking through any problems we might face, or features we might want to add.
Don't get me wrong, building tools with AI is much faster and some would say is more productive... but what happens to our skillset 10 years from now when all we've been doing is prompt engineering?

## What is a URL Shortener?

A URL shortener is a tool that turns a long, complex website address into a short, simple link that still goes to the exact same page.

Have you ever been browsing a web-page and tried to copy a link and then saw something like this:

- `https://www.google.com/search?q=what+is+a+url+shortener&oq=what+is+a+URL+shortener&gs_lcrp=EgZjaHJvbWUqDQgAEAAYkQIYgAQYigUyDQgAEAAYkQIYgAQYigUyCAgBEAAYFhgeMggIAhAAGBYYHjIICAMQABgWGB4yCAgEEAAYFhgeMggIBRAAGBYYHjIICAYQABgWGB4yCAgHEAAYFhgeMggICBAAGBYYHjIICAkQABgWGB7SAQg1NzE5ajBqN6gCALACAA&sourceid=chrome&source=chrome.ob&ie=UTF-8`

This definitely does not look pretty, and you'd be surprised how many websites contain URL's like this. Part of building a URL shortner is to also understand the problem that is being solved by a URL shortner.

## The Problem

A URL shortnener solves the problem of long, complex, and messy web addresses that are hard to share, remember, or fit into character-limited spaces.

- **Length and Character Limits**: Long links take up too much space on platforms with strict character limits like social media posts or SMS messages.
- **Aesthetic Clutter**: Clunky links with long tracking codes look messy and untrustworthy in emails, print materials, or presentations.
- **Difficulty Remembering**: Compact aliases are much easier for people to read, type manually, or recall than a chaotic string of parameters.
- **Lack of Tracking**: Standard links do not provide built-in ways to measure audience engagement, whereas many shortening services offer data on click counts, devices, and referral sources.

## How do URL Shorteners Work?

In summary, a url shortener works by taking a long web address/link, saving it in a database, and creating a short, unique code that redirects your browser to the original destination.
