For /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set mydate=%%c_%%a_%%b)
For /f "tokens=1-2 delims=/:" %%a in ("%TIME: =0%") do (set mytime=%%a%%b)
dotnet ef  --configuration Release  --startup-project ../Banking.Api/ migrations add V%mydate%_%mytime% --context Banking.Infrastructure.DbContexts.BankingDbContext
pause





