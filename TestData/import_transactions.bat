@echo off
echo Running IMPORT Transactions collection...

newman run ImportTransaction_API_Test.postman_collection.json ^
  --environment GetTransactions_Enviroment.postman_enviroment.json ^
  --reporters cli,html ^
  --reporter-html-export newman/newman-report-import-transactions.html ^
  --insecure

echo Done!
pause