### Reporting module spec.

Goal - Collect and show analytics data to the UI.

1 - Add new menu in sidebar - reporting.
clicking on it will redirect the user to the page with reportings.

page with reporting will use the same design language as the page for  /technical-support/tickets, with tabs, title, subtitle.

this page is shown only to users that have can manage assets role.

first tab is - Inventory.

The asset module will have an API to expose analyutics data:
- current inventory count - this will be shown as a simple number.
- total invetnrory price - asset value, price is summed up by currency, only in MDL. Conversion is done from usd to mdl - usd * 17.4, but from eur to mdl - eur * 20.
- number of items per asset type. This will be a piechart.
- show a graph with lines with invetory value for the whole timeline. starting with first ever asset and ending with today. split it by month. invetory price is calclated active or new (not retired) assets before 1st of the next month sum their price and this is the invetory value for that month. Do that for all months from first month that has ever had an asset attached to the last current month
- number of assets per state (e.g. active, in use etc)

- number of assets allocated (to either users or rooms) and number of asset unallocated in a piechart

Then 2nd tab is IT Support
- a graph with how many tickets were created per day and resolved per day, different color lines, line with x being days and y being number
- piechart of new or in progress tickets - low vs critical vs hijgh vs medium
- 
