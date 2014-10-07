Mere
====

Mere is an "ORM-ish thing" for SQL server. 

Getting stared
--------------
Install using nuget.
Install-Package Mere

Simple Query - using attribute for conn. config
-----------
###Model to use
  [MereTableAttribute([TableName:string, [DatabaseName:string], [ServerName:string], [UserId:string], [Password:string], [Timeout:int])]
  public class Person
  {
    public int PersonId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime Dob { get; set; }
  }
  
###Creating a query from class and filling (IEnummerable<Person> or List<Person>) where Dob is 9/15/1986
  var dob = new DateTime(1986, 9, 15);
  var results = MereQuery.Create<Person>()
    .Where(x => x.Dob).EqualTo(dob)
    .Execute();
    
  var resultsAsList = MereQuery.Create<Person>()
    .Where(x => x.Dob).EqualTo(dob)
    .ExecuteToList();
    
    
###Creating a query from class and filling a Person where first name is Jimbob
  var results = MereQuery.Create<Person>()
    .Where(x => x.FirstName).EqualTo("Jimbob")
    .ExecuteFirstOrDefault();

This will inject a "TOP 1" into your query pulling only the first record and returning it, 
if no record is found it will return null.

###Creating a query from class and IEnummerable<Person> where Dob is 9/15/1986 and first name starts with K
  var dob = new DateTime(1986, 9, 15);
  var results = MereQuery.Create<Person>()
    .Where(x => x.Dob).EqualTo(dob)
    .And(x => x.FirstName).StartsWith("K")
    .Execute();

Query - 
SELECT PersonId, FirstName, LastName 
FROM Person
WHERE Dob=@Dob1
AND FirstName LIKE @FirstName1

Query params - @Dob1=9/15/1986
@FirstName1='K%'

###Available operators
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

###AND/OR grouping example
  ####Creating a query from class and IEnummerable<Person> where Dob is 9/15/1986 and (first name starts with K or last name contains T)
  var dob = new DateTime(1986, 9, 15);
  var results = MereQuery.Create<Person>()
    .Where(x => x.Dob).EqualTo(dob)
    .AndGroup(x => x.FirstName).StartsWith("K")
    .Or(x => x.LastName).Contains("T")
    .Execute();

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
