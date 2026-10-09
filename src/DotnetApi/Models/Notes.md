# Models and DTOs

---

#### What is a Model?

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; A model is similar to a type in TypeScript. It defines
the field a specific entity has, allowing the developer to define field types, whether 
it's a nullable field, and apply any custom attributes such as [Required].

---

#### What is a DTO?

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; A DTO (Data Transfer Object) is typically related to a
ViewModel and typically relates to how HTTP requests and and responses should be formatted
when building an API. For example, the full user ViewModel might contain a UserId, but
when a client makes a request to create a user there won't be an ID available yet, so 
we define a "CreateUser" DTO that mimics the user ViewModel, but omits the UserId field.
```
// User ViewModel
public class UserViewModel {
    public Guid Id { get; set; }
    public string Name? { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool IsActive { get; set; } = true;
}

// Create User DTO
public class CreateUserDto {
    public string Name? { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
```
In the above example, we set `Name` as a nullable field,
give `IsActive` a default value of `true`, and define a Create User DTO omitting 
certain fields. If a name is not provided when the client calls the `createUser`
endpoint, the type default value will be applied (null in this case, 
not `string.Empty` as strings are reference types).

---

### Attributes

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Attributes are essentially tags applied to fields that
provide additional details, add restrictions, and can apply validation. Some of the most
common attributes are:

#### Required

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; By adding the `[Required]` attribute to a model or DTO,
the API middleware can catch missing fields in payloads and return 400 errors before the
request even reaches the controller. For example, if we mark the password as required on 
the create user DTO above, any requests forgetting it will have a 400 error returned from the
middleware. On this early return, we can set the error message right on the attribute:
```
public class CreateUserDto {
    public string Name? { get; set; }
    public string Email { get; set; }
    
    // Note: composite strings on StringLength use 0 as the 
    // field name, 1 as the max length, and 2 as the min length
    [Required(AllowEmptyStrings = false, ErrorMessage: "Password is required.")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "{0} must be at least {2} characters long.")]
    public string Password { get; set; }
}
```