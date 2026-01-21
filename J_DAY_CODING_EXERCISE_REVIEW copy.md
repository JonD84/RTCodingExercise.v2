# Coding Exercise Review

## Overview

I opted for Challenge 1 and did not fully complete. I spent aprox 5hrs getting my tooling sorted, with various vs code / Mac issues. I then spent a further 6hrs writing code and tests. 

I did not accompish what I planned to in that time, but you will see that I added a new button to the homepage reg items, to allow a user to add items to a watchlist table, I then added a watchlist page, which used a table to display the plates added. It allows the user to set a watch price, which is persisted in the new table i created (using code first) and also remove the items from the list. I added some basic tests for both frontend and service, which all pass. But did not manage to hook up events to the watchlist page, I only worked on the MVC page, there are no changes to the Angular project. 

Futher tasks I would add would be to highlight that row to the user as part of the notificaion and consider how this event could be used to notify the user either via email or text message. This would require a communcation choise in the user profile and other settings like mobile infomation and device uuid. Another thing I would like to add is some ML model with forcasing of demand for the plate to the user based on historical data and suggestions on demand. From a business prefective to add some "Urgency" to making a purchase, I would add how many other watchers a plate has. though this could have a negitive effect with some plates not having many or any watchers! 
 