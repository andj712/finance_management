@echo off
echo Running GET Transactions collection...

newman run GetTransactions_API_Test.postman_collection.json ^
  --environment GetTransactions_Enviroment.postman_enviroment.json ^
  --reporters cli,html ^
  --reporter-html-export newman/newman-report-get-transactions.html ^
  --insecure

echo Done!
pause