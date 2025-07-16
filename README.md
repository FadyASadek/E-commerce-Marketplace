

 A-Store 

More than just a store. It's a full-stack, multi-vendor e-commerce powerhouse built from the ground up with ASP.NET Core MVC.

This isn't your typical tutorial project. A-Store is a complete, real-world marketplace designed to handle the complex interactions between customers, sellers, and administrators. It's a comprehensive platform where sellers get their own dashboards to manage their business, customers get a seamless shopping experience, and admins have total control over the entire ecosystem.

---

 So, What Can It Do? 

This project is packed with professional features, broken down by who gets to use them.

 For Customers (The Shopping Experience) 

* Seamless User Auth: Register and log in with a local account or just use your Google account.
* Dynamic Product Catalog: A powerful search and filtering system to find exactly what you're looking for.
* A Snappy Shopping Cart: Add, update, or remove items instantly with AJAX. No page reloads, just pure speed.
* Real-Time Stock Validation: No more ordering products that are out of stock. The system checks availability on the fly.
* Secure Checkout: A clean, multi-step checkout process with a "Cash on Delivery" option.
* Your Own Dashboard: Track your order history, view details, and even cancel pending orders yourself.

### For Sellers (Your Own Business Portal)

* Dedicated Seller Dashboard: Get a bird's-eye view of your sales with key stats: Total Revenue, Items Sold, and more.
* "My Sales" Page: A focused view to manage orders for your products only.
* Full Order Control: Update the status of your orders from "Pending" all the way to "Shipped" to keep customers informed.

 For Admins (The God Mode) 

* The Command Center: A comprehensive Admin Dashboard with site-wide statistics and control.
* User Management: List all users, create new accounts with specific roles, and Block/Unblock anyone on the platform.
* Role Management: Don't just assign roles, create new ones. Need a "Moderator" role? Go for it.
* Total Order Oversight: View, filter, and search every single order in the system. Manually update any order's status when needed.
* Full Catalog Control: Complete CRUD (Create, Read, Update, Delete) functionality for all Products and Categories.
* Smart Soft Deletes: Products are never truly gone. They're soft-deleted to maintain the integrity of past orders and sales data.

---

  The Tech & The Brains Behind It 

This project is built on a modern, robust, and scalable tech stack.

* Backend: ASP.NET Core MVC, C#, .NET, Entity Framework Core, ASP.NET Core Identity.
* Frontend: Razor Pages, HTML5, CSS3, JavaScript, jQuery, AJAX.
* Database: Microsoft SQL Server.
* Sweet Alerts: Because default browser alerts are ugly. We use SweetAlert2 for clean, modern notifications.
* Architecture & Patterns:

  * Repository & Unit of Work Patterns: Because clean, testable controllers are non-negotiable.
  * Rock-Solid MVC Architecture: For a crystal-clear separation of concerns.
  * Role-Based Authorization: Securely locking down parts of the app. Admins see admin things, sellers see seller things. Simple as that.
  * Database Transactions: Ensuring that critical operations, like placing an order, either succeed completely or fail safely without corrupting data.

---

 Get It Running 

Ready to see it in action? Follow these steps.

1. Clone the repository:

   ```bash
   git clone [your-repo-url]
   ```
2. Make sure you have the .NET SDK and SQL Server installed and running.
3. Set up your connection string and Google Authentication keys using the User Secrets manager in Visual Studio (Right-click project > Manage User Secrets).
4. Open the Package Manager Console and run the `update-database` command to let EF Core build the database for you.
5. Press F5 and watch the magic happen.

 

