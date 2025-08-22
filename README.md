Summary of issues and how to resolve:

DonationController.cs:
- Every task returns Ok unconditionally. We need to handle the event in which something fails. Furthermore, when we `CreateDonation` we're returning `Ok` with an object that contains the success status (which is also hard-coded to `true`). Presumably we should instead have a conditional here which checks the success status (and that success status should come from `result`; the message should not come from result and can instead be defined here).

DonationRepository.cs:
- Insecure password (ideally it should be randomly generated to mitigate brute force attacks).
- Password is in plaintext in the code (in a production setting we should use a Key Vault (such as Azure Key Vault), whereas in a development setting we may be able to use environment variables, a secret manager tool, or configuration files, though in the case of configuration files care must be taken to ensure they are never committed to the repository; though in each of these cases the password is still stored locally thus it is not suitable for production).
- Vulnerable SQL statements; we should never concatenate user entered data with an SQL statement we should instead use parameterized queries to sanitize user input and prevent the possibility of an SQL injection attack.
- `SaveDonation` should return a boolean as to the donation success status (and should be equipped to handle SQL exceptions and return false in the event of an exception). In general, it should not be the responsibility of SaveDonation to return a message, it should only return a success status.

DonationService.cs:
- I don't think we should be using delays here. While it is non-blocking (and so the thread is free to do other tasks), it seems unnecessary and ultimately only serves the slow down the DonationService.
- We seem to have two different copies of DonationService.cs, one in the Services folder and one in the root. The DonationService.cs in the root should be removed and we should ensure every reference to it refers to Services/DonationService.cs instead.

Changes made:
- Changed `SaveDonation` to return a bool and to use an SQL Transaction, as well as parameterized commands. This way we avoid the potential for an SQL injection attack and we ensure that our donation is saved properly (and if something goes wrong while saving, we roll back the transaction and return false to report that there was a failure with saving the donation).
- To accomodate those changes to `SaveDonation` I have also changed `CreateDonation` to use a conditional; if our donation fails to save then `CreateDonation` will return `BadRequest` with the message "Failed to process donation". The success parameter has also been removed from the return object as returning a `BadRequest` implies that there was no success.
- Removed the copy of `DonationService` from the project root; we should only be referencing the version in the Services folder.