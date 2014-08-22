USE [FrameLog.Example.ExampleContext];

SELECT * FROM Books;

SELECT ChangeSets.Id, Users.Name AS Author, Timestamp, TypeName, ObjectReference, PropertyName, Value
FROM ChangeSets
INNER JOIN ObjectChanges ON ChangeSets.id = ObjectChanges.ChangeSet_Id
INNER JOIN Users ON ChangeSets.Author_Id = Users.Id
INNER JOIN PropertyChanges ON ObjectChanges.Id = PropertyChanges.ObjectChange_Id
ORDER BY Timestamp DESC;
