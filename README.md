#Mere

Mere is an "ORM-ish thing" for SQL server. 

##Getting stared

Install using nuget.
Install-Package Mere

##Note that all executes have the equivalent async option available to take advantage of "TPL"

##Statement types
#####Ones that use the fluent flitering api
  ```c#
  var query = MereQuery.Create<Person>();
  var update = MereUpdate.Create<Person>();
  var delete = MereDelete.Create<Person>();
  ```
#####Ones that do not 
  ```c#
  var insert = MereInsert.Create<Person>();
  ```
##Simple Queries - using attribute for conn. config

###Model to use
  ```c#
  [MereTableAttribute([TableName:string, [DatabaseName:string], [ServerName:string], [UserId:string], [Password:string], [Timeout:int])]
  public class Person
  {
    public int PersonId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Dob { get; set; }
  }
  ```
  
#####Creating a query from class and filling (IEnumerable<Person> or List<Person>) where Dob is 9/15/1986
  ```c#
  var dob = new DateTime(1986, 9, 15);
  var results = MereQuery.Create<Person>()
    .Where(x => x.Dob).EqualTo(dob)
    .Execute();
    
  var resultsAsList = MereQuery.Create<Person>()
    .Where(x => x.Dob).EqualTo(dob)
    .ExecuteToList();
  ```
    
    
#####Creating a query from class and filling a Person where first name is Jimbob
  ```c#
  var results = MereQuery.Create<Person>()
    .Where(x => x.FirstName).EqualTo("Jimbob")
    .ExecuteFirstOrDefault();
  ```

This will inject a "TOP 1" into your query pulling only the first record and returning it, 
if no record is found it will return null.

#####Creating a query from class and IEnumerable<Person> where Dob is 9/15/1986 and first name starts with K
```c#
var dob = new DateTime(1986, 9, 15);
var results = MereQuery.Create<Person>()
  .Where(x => x.Dob).EqualTo(dob)
  .And(x => x.FirstName).StartsWith("K")
  .Execute();
```

Query - 
SELECT PersonId, FirstName, LastName 
FROM Person
WHERE Dob=@Dob1
AND FirstName LIKE @FirstName1

Query params - @Dob1=9/15/1986
@FirstName1='K%'

#####Available operators
EqualTo

EqualToCaseSensitive

NotEqualTo

NotEqualToCaseSensitive

GreaterThan

GreaterThanOrEqualTo

LessThan

LessThanOrEqualTo

In

InCaseSensitive

NotIn

NotInCaseSensitive

Between

NotBetween

Contains

ContainsCaseSensitive

NotContains

NotContainsCaseSensitive

StartsWith

StartsWithCaseSensitive

NotStartsWithCaseSensitive

EndsWith

EndsWithCaseSensitive

NotEndsWith

NotEndsWithCaseSensitive


#####AND/OR grouping example
#####Creating a query from class and IEnumerable<Person> where Dob is 9/15/1986 and (first name starts with K or last name contains T)
  ```c#
  var dob = new DateTime(1986, 9, 15);
  var results = MereQuery.Create<Person>()
    .Where(x => x.Dob).EqualTo(dob)
    .AndGroup(x => x.FirstName).StartsWith("K")
    .Or(x => x.LastName).Contains("T")
    .Execute();
  ```

Query - 
SELECT PersonId, FirstName, LastName 
FROM Person
WHERE Dob=@Dob1
AND (
FirstName LIKE @FirstName1
OR LastName LIKE @LastName!
)

Query params - @Dob1=9/15/1986
@FirstName1='K%'
@LastName1='%T%'


##Some more examples
###Model to use
  ```c#
  [MereTableAttribute([TableName:string, [DatabaseName:string], [ServerName:string], [UserId:string], [Password:string], [Timeout:int])]
  public class Person
  {
    [MereIdentity]
    [MereKey]
    public int PersonId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Dob { get; set; }
  }
  ```

#####Instantiating filling and upserting new person record
  ```c#
  var p = new Person
    {
      FirstName = "Jimbob",
      LastName = "Jones",
      Dob = new DateTime(1986, 9 ,15)
    };

    p.MereSave();

    var newId = p.PersonId;//this will be automatically set to the value per the @@IDENTITY value of the transaction
  ```
#####Pulling person updating property and upserting new person record
  ```c#
  var p = MereQuery.Create<Person>()
    .Where(x => x.FirstName).EqualTo("Jimbob")
    .ExecuteFirstOrDefault();

    //know that p could be null

    p.LastName = "Johnson";

    p.MereSave();

    var newId = p.PersonId;//this will be what ever id was pull on the initial query
  ```

#####Copying data from one server to another
    ```c#
    var ds1 = MereDataSource.Create([ServerName1:string], [DatabaseName1:string], [UserId1:string], [Password1:string]);
    var ds2 = MereDataSource.Create([ServerName2:string], [DatabaseName2:string], [UserId2:string], [Password2:string]);

    //createing a MereQuery with a MereDataSource will over attribute conn. config 
    var sourceData = MereQuery.Create<Person>(ds1)
      .Execute();//this will be an IEnumerable<Person> from server 1)

    sourceData.MereBulkCopy(ds2);//this will insert the data from server1 into server2 taking advantage of sql bulk abilities
    ```

#####Executing custom sql
```c#
var sql = @"SELECT FirstName, LastName FROM Person";
```
  ```c#
  var sql = @"SELECT FirstName, LastName FROM Person";
  var results = MereQuery.Create<Person>.ExecuteCustomQuery(sql);// will be IEnumerable<Person> with only FirstName and LastName values set
  
  //execute sql params
  var sqlp = @"SELECT FirstName, LastName FROM Person WHERE FirstName=@fName";
  var resultsp = MereQuery.Create<Person>.ExecuteCustomQueryToList(sql, new {fName="Jimbob"});// will be List<Person> with only FirstName and LastName values set
  
  //using MereUtils short hand version
  var resultsS = MereUtils.ExecuteQuery<Person>(sql);
  var resultspS = MereUtils.ExecuteQuery<Person>(sqlp, new {fName="Jimbob"});
  ```

#####Truncating a table
  ```c#
    MereUtils.TruncateTable<Person>();
    var p = new Person();
    p.TruncateTable();
  ```
