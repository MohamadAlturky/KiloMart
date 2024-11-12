- The user submits an order, which includes product IDs and quantities (optionally discount codes), and the system saves it with an "initialized" status.

- If the stock didn't have the required quantity, make the order failed and return the failed message to the user.

- in this stage if the user canceled the order the sytem will change the status to 'canceled by user before the provider accepted it'

If the order contains products from multiple providers, the system will respond with an error message if the admin chooses to restrict mixed-provider orders. Alternatively, if the admin allows the best-provider-matching-algo, the system will remove products from other providers and only proceed with products from a single provider and notify the user.

the provider either accepts the order or decline it.
- if decline the status will be declined from provider
- else the status is accepted from  provider

if the order is declined from the customer 
