# A price scraper meant for comparing prices of electronics stores. Written using C#, cshtml, Python(for the scrapper part) and a lot of coffee.


## What it can do already:

1. It can get the data from Topocentras and Avitela, then display it in a website and sort by price.
2. It can save the data to a database, so that the Python web scrapper doesn't need to run everytime the same thing is searched. It only runs if the data is at least 2 hours old on that product.
3. It saves and displays a users search history for the user himself.

## The project itself isn't finished completely and could use some improvements:

1. Should rewrite the scraping itself from Python to C#, because Python sometimes doesn't want to get the data from the websites and just breaks.
2. Should restructure the code to make it cleaner and more easy to read.
3. Should move from a Mysql database to MongoDB, so that we could easily use the data that we receive from any Electronics sale website we want.
4. The design could be made quite a bit better. It's just a placeholder design, because we needed the functionality and not the looks themselves
