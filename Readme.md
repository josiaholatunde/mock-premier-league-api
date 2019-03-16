## Mock Premier League Api
#This api displays the latest scores ofa mock premier league and gives users and Admins certain rights
#These are the end points to access api resources

#Base Url : https://premier-league.azurewebsites.net/api/

#Description for endpoint route
<pre>
GET /api/users -displays the list of existing users within the application <br>
POST /api/users/MakeUserAdmin -an endpoint ton make an existing user an admin (requires Admin Access) <br/>
POST /api/auth/login - endpoint to login for both users and admin (requires username and password in
the body of request) <br>
PUT /api/auth/register - endpoint to register for a non user<br><br><br>


GET /api/teams -displays the list of teams within the mock premier league and can be viewed by logged 
in users and admins<br>
POST /api/teams - creates a new team resource (Admin access)<br>
PUT /api/teams/:id - updates an existing team (Admin Access)<br>
DELETE /api/teams/:id -deletes an existing team resorce (Admin Resource) <br><br><br>


GET /api/fixtures -displays the list of fixtures within the mock premier league and can be viewed by 
logged in users and admins<br/>
POST /api/fixtures - creates a new fixture resource (Admin access)<br>
PUT /api/fixtures/:id - updates an existing fixture (Admin Access)<br>
</pre>
