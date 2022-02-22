module GroupUp.Domain

open System
open GroupUp.BaseTypes

type GroupUpCreated = {Id: Guid; Date: DateTime; Name: String50; MaxAttendants: int}