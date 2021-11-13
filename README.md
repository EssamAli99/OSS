# OSS
OSS is a simple framework to build .net core web applications.
Trying to provide basic functions needed in any modern application.
The project provide functions like
<ul>
  <li>Login/Register pages (default .net identity)</li>
  <li>Logging errors to database</li>
  <li>Exception handling</li>
  <li>Caching</li>
  <li>Validation (using fluent Validation)</li>
  <li>Email Queue</li>
  <li>Sending Email (using MailKit)</li>
  <li>Schedule Tasks (tasks configured in database)</li>
  <li>Localization (using database)</li>
  <li>Flixable way to handle permissions (admin can create roles and give different permissions on any page)</li>
  <li>Unit test</li>
</ul>


I used <a href="https://www.nopcommerce.com/en">nopCommerce</a> as a reference, and tried to make my code much simpler. The code is not complete<br/>
I included the database files (in folder App_Data) to be easier to attach, you need to change the connection string in the startup.cs file.
<br/>
Login user name: <b>aaa@test.com</b> and password <b>aaa123</b>

