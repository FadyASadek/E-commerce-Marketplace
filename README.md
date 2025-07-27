# A-Store: A Feature-Rich Multi-Vendor E-commerce Marketplace

A complete, full-stack e-commerce marketplace built from scratch using ASP.NET Core MVC. This platform is designed to connect multiple sellers with customers, allowing sellers to manage their own products and sales, while customers enjoy a seamless shopping experience. The entire system is overseen by a comprehensive admin dashboard.


##  Key Features

This project is packed with professional, real-world features:

####  **For Customers:**
-   User Registration & Login (including Google external login).
-   Dynamic Product Catalog with searching and filtering.
-   Interactive Shopping Cart (Add/Update/Remove items with AJAX).
-   Real-time stock validation to prevent ordering unavailable products.
-   Secure, multi-step Checkout Process with "Cash on Delivery" option.
-   Personal Dashboard to view "My Orders" history and details.
-   Ability to cancel pending orders.

####  **For Sellers:**
-   Dedicated Seller Dashboard with sales statistics (Total Revenue, Items Sold, etc.).
-   A "My Sales" page to view and manage orders for their products only.
-   Ability to update order status (e.g., from "Pending" to "Processing" to "Shipped").

####  **For Admins:**
-   A comprehensive Admin Dashboard with site-wide statistics.
-   **User Management:** List all users, create new users with specific roles, and Block/Unblock user accounts.
-   **Role Management:** View and create new user roles (e.g., Admin, Seller).
-   **Order Management:** View all orders in the system, with filtering and search capabilities. Ability to manually update any order's status.
-   **Catalog Management:** Full CRUD (Create, Read, Update, Delete) functionality for Products and Categories.
-   **Soft Delete:** Products are "soft-deleted" to maintain historical data integrity for past orders.

---

##  Technology Stack & Architecture

-   **Backend:** ASP.NET Core MVC, C#, .NET, Entity Framework Core, ASP.NET Core Identity.
-   **Frontend:** Razor Pages, HTML5, CSS3, JavaScript, jQuery, AJAX.
-   **Database:** Microsoft SQL Server.
-   **Libraries:** SweetAlert2 for modern and responsive user notifications.
-   **Architectural Patterns:**
    -   **Repository Pattern :** To abstract data access logic and maintain clean, testable controllers.
    -   **MVC (Model-View-Controller):** For a clear separation of concerns.
    * **Role-Based Authorization:** Securely restricting access to different parts of the application based on user roles (Admin, Seller, Customer).
    * **Database Transactions:** Ensuring data integrity for critical operations like placing an order.

---

##  How to Run Locally

1.  Clone the repository: `git clone [your-repo-url]`
2.  Ensure you have the **.NET SDK** and **SQL Server** installed.
3.  Set up your connection string and Google Authentication keys using the **User Secrets** manager in Visual Studio (`Right-click project > Manage User Secrets`).
4.  Open the Package Manager Console and run `update-database` to create the database schema.
5.  Run the project (F5).
