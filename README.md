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

In summary, a url shortener works by taking a long web address/link, saving it in a database, and creating a short, unique code that redirects your browser to the original destination. So when you take your long URL and paste it into a URL shortening website, these non-exaustive steps take place.

- Receive the long url
- Generate a unique code for the long url
- Save the long url + code into the database
- Return the domain + code as a response (e.g. `https://tinyurl.com/uiHUSIdh`).

In this case, the `https://tinyurl.com/` part of the url is the domain, and the `uiHUSIdh` is the short code that maps this short url to the long url that a user will eventually want to go to.

**What happens when the short url is entered into a browser?**

When the short url is entered into a brower, the application will then follow these non-exaustive steps:

- Receive the short url from the browser
- Find the long-url that the short-url maps to
- Return the long-url as part of a 301/302 redirect to the browser.

Since this is a 301/302 redirect, the browser will automatically redirect the user to the target destination; which in out case will be the long-url.

### 301 vs 302 Http Redirects

A 301 redirect is used to make sure search engines and users are sent to the correct page. A 301 status code is used when any page has been **permanently**  moved to another location. Users will now see the new URL as it has replaced the old page. This will change the URL of the page when it shows in search engine results.

301 status codes should be used under these circumstances:

- Links to any outdated URLs need to be sent to your desired page. A case in point would be the merging of two webpages or websites that are migrating permanently.
- There are several URLs used to access your site. Select a single URL as a canonical and preferred destination and use your 301 redirects to direct traffic to the preferred URL or the new one.
- You've moved your site to a new domain name, and you want to make the transition from your old site to your new website as seamless as possible.
- You are conducting an http to https migration.

_Since it is permanently redirected, the browser caches the response, and subsequent requests for the same URL will not be sent to the URL shortening service. Instead, requests are redirected to the long URL server directly._

A 302 redirect is a temporary redirect and directs users and search engines to the desired page for a limited amount of time until it is removed. It may be shown as a 302 found (HTTP 1.1) or moved temporarily (HTTP 1.0).

There are times when a 302 redirect is useful. Add a 302 redirect for:

- A/B testing of a webpage for functionality or design.
- Getting client feedback on a new page without impacting site ranking.
- Updating a web page while providing viewers with a consistent experience.
- Broken webpage and you want to maintain a good user experience in the meantime.

302 are temporary redirects and used when webmasters need to assess performance or gather feedback. They are not to be used as a permanent solution.

For our use case we will make use of **301 redirects** as for a provided URL we will require a permanent redirect to the destination long-url. This will also allow us to make use of browser caching so the same user will not hit our server multiple times for the same short URL.
