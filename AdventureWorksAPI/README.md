AdventureWorksAPI

PART 1

1. How to run
- Clone this repository
    git clone https://github.com/linhcscscs/AdventureWorksApi

- Restore AdventureWorks2022 database on your SQL Server.

- Open solution AdventureWorksAPI.sln in Visual Studio.

- Update connection string in appsettings.json, ví dụ:
    "ConnectionStrings": {
        "AdventureWorks": "Server=localhost\\SQLEXPRESS;Database=AdventureWorks2022;Trusted_Connection=True;TrustServerCertificate=True;"
    }

- Run the project (F5).

- Access API documentation & try endpoints via Swagger:
    https://localhost:44321/swagger/index.html


#region Section 1 - Low Stock API

Low Stock Alert API
- Endpoint:
    GET /LowStockApi

Query Parameters
------------------------------------------------------
Name    Type                Required    Example       Description
------  ------------------  ---------   ------------  --------------------------
date    string (yyyy-MM-dd)    Yes       2011-01-01    Date used as alert reference point

Response
------------------------------------------------------
Status 200 - OK
Returns list of products that are potentially at low stock based on average sales over the past 3 months.

Status 204 - No Content
If no products match the low stock alert criteria.

Status 500 - Server Error
Returned if an unhandled exception occurs.

#endregion


#region Section 2 - Monthly Report API

Monthly Report API
- Endpoint:
    GET /MonthlyReportApi

Query Parameters
------------------------------------------------------
Name       Type   Required  Example  Description
---------  -----  --------  -------  -----------------------
fromMonth   int     Yes       1       From month (1-12)
fromYear    int     Yes       2011    From year
toMonth     int     Yes       3       To month (1-12)
toYear      int     Yes       2011    To year

Response
------------------------------------------------------
Status 200 - OK
Returns list of monthly reports.

Status 204 - No Content
If no data exists in the specified range.

Status 500 - Server Error
For unexpected server errors.

#endregion

#region Section 3 - Order API

Order Management API

Base route: /api/OrderApi

------------------------------------------------------
1. Create Order
- Endpoint: POST /api/OrderApi
- Request body:
{
    "customerID": 123,
    "totalDue": 1000.50,
    "status": "Pending",  // enum: Pending=1, Approved=2, Backordered=3, Rejected=4, Shipped=5, Cancelled=6
    "orderDetails": [
        {
            "productID": 680,
            "orderQty": 2
        },
        {
            "productID": 712,
            "orderQty": 1
        }
    ]
}
- Response:
Status 200 - OK
{
    "orderID": 456
}
Status 500 - Server Error


------------------------------------------------------
2. Get Orders By Customer
- Endpoint: GET /api/OrderApi/GetOrdersByCustomer/{customerId}
- Example: /api/OrderApi/GetOrdersByCustomer/5
- Response:
Status 200 - OK
[
    {
        "orderID": 101,
        "customerID": 5,
        "totalDue": 500.00,
        "status": "Pending",
        "orderDetails": [
            {
                "productID": 680,
                "orderQty": 2
            }
        ]
    },
    {
        "orderID": 102,
        "customerID": 5,
        "totalDue": 300.00,
        "status": "Shipped",
        "orderDetails": [
            {
                "productID": 712,
                "orderQty": 1
            }
        ]
    }
]
Status 204 - No Content
Status 500 - Server Error


------------------------------------------------------
3. Update Order Status
- Endpoint: PUT /api/OrderApi/UpdateOrderStatus/{orderId}
- Example: /api/OrderApi/UpdateOrderStatus/10
- Body:
"Approved"  // (or any enum value: Pending, Approved, Backordered, Rejected, Shipped, Cancelled)
- Response:
Status 200 - OK
Status 404 - Not Found
Status 500 - Server Error

