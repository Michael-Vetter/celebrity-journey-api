# celebrity-journey-api
backend for celebrity-journey.com
The code runs in an AWS Lambda function.

# About this app
This API provides all the content for site celebrity-journey.com.  The UI is here: https://github.com/Michael-Vetter/celebrity-journey-ui

# Help needed
This originally started as an API to add data to a log in an AWS S3 bucket.  Then the content on my site, which I stored in json files, grew too big and was slowing the site down.  I still maintain the data in json files.  The reason for this is 1) I have almost no visitors to my site, and 2) databases in AWS are not cheep.  Eventually I may switch to that, but it is currently not my priority.  The code could use some refactoring to make it fully testable locally. I started down that path, but still have a ways to go.  Previously I would just publish directly to AWS and test, but that was becuase I was not worried about breaking anything.

