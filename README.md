# DTORepository
DTORepository is a library use to facilitate mapping a DTO to/from an Entity in EntityFramework with generic repository pattern. By enabling a developer to put most of business DTO-related logic inside an DTO class, this library enhances the source code's maintainability and readability.
### Installation

Nuget Package Manager: 
**DTORepository**

or run command in Package Manager Console: 
`PM> Install-Package DTORepository`
# Features
  - Automatically convert a DTO to an Entity and store in the datasource and vice-versa.
  - Conversion logics when writing DTO into database (create & update)
  - Projection logic when retrieving from database to DTO
  - Determine which fields to be ignore from writing/retrieving to/from the datasource
  - Handle many-to-many, one-to-many relationship.
  - Transaction supported (Unit of Work)
  - Repository pattern
  - Can be easily used with Dependency Injection library (eg. Autofac)

# How to use
For example, you have these EntityFramework's entities
```csharp
class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
```
## Create a Repository and a DTO for CRUD operation
##### Create a Repository
To perform any action on the datasource, this library encourages the repository pattern to be used. Each repository will bind with only one entity. Then, a user can perform any CRUD operation on this repository via Data Transfer Object(DTO).
```csharp
    RepositoryFactory repositoryFactory = new RepositoryFactory(yourDbContext);
    IRepository<User> repository = repositoryFactory.CreateRepository<User>();
```
##### Create a DTO
The DTOs is the heart of this library. To avoid having a lot of complicated logic and unmanaged source code, we can now push all DTO-related logic inside the DTO. First, Let start with a very simple DTO.
```csharp
class UserDto : DTORepository.Models.DtoBase<User, UserDto>
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
}
```
## Create new User from a UserDTO
To create an object from dto
```csharp
UserDto dto = new UserDto {
    UserName = "johnd",
    Password = "pwd123",
    FirstName = "John",
    LastName = "Doe",
    Age = 23
};
ISuccessOrErrors<UserDto> status = repository.Create(dto);
UserDto createdDto = status.Result;
Assert.Equal(dto, createdDto); // dto can be accessed to get the newly created identifier
```

## Update user from UserDTO
Since  we already have a user in the database (assume its Id is 1)

```csharp
UserDto dto = new UserDto {
    Id = 1,
    UserName = "tonys",
    Password = "123pwd",
    FirstName = "Tony",
    LastName = "Slark",
    Age = 36
};
ISuccessOrErrors<UserDto> status = repository.Update(dto);
UserDto updateDto = status.Result;
Assert.Equal("tonys", updateDto.UserName);
```

#### Ignore fields from creating or updating
If we want to update only some fields, by default the library will ignore mapping a field with null value. in DTO to entity. However, the Age field has a type of int (it cannot be set to null). So, we can change the field type of Age to int? or Nullable<int>, or we can use an Attribute called ***IgnoreMappingIfValueAttribute*** to specify the ignored value
```csharp
class UserDto : DTORepository.Models.DtoBase<User, UserDto>
{
    //...
    public int? Age { get; set; }
    // or
    public Nullable<int> Age { get; set; }
    // or
    [IgnoreMappingIfValue(0)]
    public int Age { get; set; }
    //...
}
UserDto dto = new UserDto {
    Id = 1,
    FirstName = "Tony"
};
ISuccessOrErrors<UserDto> status = repository.Update(dto);
UserDto updateDto = status.Result;
Assert.Equal("Tony", updateDto.FirstName);
Assert.Equal("johnd", updateDto.UserName);
Assert.Equal(23, updateDto.Age);
```

If we want to forbid a user from updating his username, password, we can use an attribute called ***IgnoreWritingForAttribute***
```csharp
class UserDto : DTORepository.Models.DtoBase<User, UserDto>
{
    //...
    [IgnoreWritingFor(ActionFlags.Update)]
    public string UserName { get; set; }
    [IgnoreWritingFor(ActionFlags.Update)]
    public string Password { get; set; }
    //...
}
```
Additionally, it can be used with a create action too.
```csharp
class UserDto : DTORepository.Models.DtoBase<User, UserDto>
{
    //...
    [IgnoreWritingFor(ActionFlags.Create)]
    public string FirstName { get; set; }
    [IgnoreWritingFor(ActionFlags.Create)]
    public string LastName { get; set; }
    [IgnoreWritingFor(ActionFlags.Create)]
    public int Age { get; set; }
    //...
}
```
Or, you can just create separate DTOs for each actions.
```csharp
class UserCreateDto : DTORepository.Models.DtoBase<User, UserCreateDto>
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}
class UserUpdateDto : DTORepository.Models.DtoBase<User, UserUpdateDto>
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int? Age { get; set; }
}
```
#### Implement logics before the data is created or updated into the database
```csharp
class UserDto : DTORepository.Models.DtoBase<User, UserDto>
{
    //...
    protected override ISuccessOrErrors<User> CreateDataFromDto(DbContext context, User target
    {
        var status = base.CreateDataFromDto(context, target);
        var user = status.Result;
        user.CreatedDate = DateTime.Now;
        return status;
    }
    protected override ISuccessOrErrors<User> UpdateDataFromDto(DbContext context, User target, User original)
    {
        var status = base.UpdateDataFromDto(context, target, original);
        var user = status.Result;
        user.UpdatedDate = DateTime.Now;
        return status;
    }
    //...
}
```

## Read entities from datasource as DTOs
### Get one by its identifiers
```csharp
var status = repository.Get<UserDto>(1);
UserDto userDto = status.Result;
```
We can add a logic when the library retrieving entities from the datasource and convert them to DTOs by overriding ***EntityToDtoMapping*** of a DTO class that inherits DtoBase
```csharp
class UserDto : DTORepository.Models.DtoBase<User, UserDto>
{
    //...
    public String Name { get; set; }
    //...
    protected override Action<IMappingExpression<User, UserDto>> EntityToDtoMapping
    {
        get
        {
            return m => m.ForMember(d => d.Name,
                opt => opt.MapFrom(s => s.FirstName + " " + s.LastName));
        }
    }
}
```
Ps. Since the conversion process from entity to dto use the AutoMapper.ProjectTo() method, the operations that can be performed in EntityToDtoMapping are limited (https://github.com/AutoMapper/AutoMapper/wiki/Queryable-Extensions#supported-mapping-options)

### List entities by predicate
```csharp
ISuccessOrErrors<IList<UserDto>> status = repository.Query<UserDto>(x => x.FirstName == "John");
IList<UserDto> userDto = status.Result;
```
### Get queryable object of DTOs
```csharp
IQueryable<UserDto> userDtoQuery = repository.List<UserDto>();
```

#### Ignore fields from reading or listing
Same as *create* or *update*, *get* or *list* can be treated as different actions. We can use
***IgnoreRetreivingForAttribute*** to specify which fields to ignore for specific actions.
```csharp
class UserDto : DTORepository.Models.DtoBase<User, UserDto>
{
    //...
    [IgnoreRetreivingFor(ActionFlags.Get | ActionFlags.List)]
    public string Password { get; set; }
    //...
    [IgnoreRetreivingFor(ActionFlags.List)]
    public int Age { get; set; }
    //...
}
```
In case of your DTO class contains lists of other DTOs as well. The fields in that nested DTOs will be ignore based on its main action. For example, considering these requirements:
- When list blogs, the number of posts for that blog are requred to be displayed
- When list posts, only the topics are required to be dispalyed

Hence, We can create 2 DTOs based on the requirements
```csharp
class BlogDto : DTORepository.Models.DtoBase<Blog, BlogDto> {
    public int Id { get; set; }
    [IgnoreRetreivingFor(ActionFlags.List)]
    public ICollection<PostDto> Posts { get; set; }
    [IgnoreRetreivingFor(ActionFlags.Get)]
    public int NumberOfPosts { get; set; }
    protected override Action<IMappingExpression<Blog, BlogDto>> EntityToDtoMapping
    {
        get
        {
            return m => m.ForMember(d => d.NumberOfPosts,
                opt => opt.MapFrom(s => s.Posts.Count()));
        }
    }
}
class PostDto : DTORepository.Models.DtoBase<Post, PostDto> {
    public int Id { get; set; }
    public string Topic { get; set; }
    [IgnoreRetreivingFor(ActionFlags.List)]
    public string Body { get; set; }
}
```
Then, perform an action to retrieve data
```csharp
IRepository<Blog> blogRepository = repositoryFactory.CreateRepository<Blog>();
// --- list action ---
IList<BlogDto> blogDtos = blogRepository.List<BlogDto>().ToList();
// blogDtos[i].Posts will be null
// blogDtos[i].NumberOfPost will have a number

// --- get action ---
var status = blogRepository.Get<BlogDto>(1);
BlogDto blogDto = status.Result;
// blogDto.Posts will be a list of PostDto
// blogDto.NumberOfPost will be zero (since it is a default value of an int)
// blogDto.Posts[i].Body will have a value because it considers the operation as Get(even the Posts field is a collection)
```
## Delete entities from datasource
For the deletion, the DTO is not required. We can just pass an entity's identifiers as arguments.
```csharp
ISuccessOrErrors status = repository.Delete(1); // delete a User entity with Id of 1
```

## Transaction
Normally, every repository operation will perform **dbContext.SaveChanges()** on itself. However, if there is a requirement to perform many operations as a transaction, we can use the library's ***UnitOfWork*** implementation to wrap operations together as a single transaction.
```csharp
//...
UnitOfWorkFactory unitOfWorkFactory = new UnitOfWorkFactory(dbContext);
UnitOfWork uow = unitOfWorkFactory.CreateUnitOfWork();
ISuccessOrErrors status = uow.Execute((dbContext) => {
    var status = repository.Create(newUserDto);
    status.Combine(blogRepository.Update(updatedBlogDto));
    status.Combine(blogRepository.Delete(blogId));
    return status;
});
Assert.True(status.IsValid); // indicate that transaction is completed
//...
```

## Dependency Injection with Autofac
In your AutoFac startup config in App_Start folder of your project, add these lines
```csharp
var builder = new ContainerBuilder();
//...
builder.RegisterType<YourContextName>().As<DbContext>().InstancePerRequest();
builder.RegisterAssemblyTypes(typeof(RepositoryFactory).Assembly).AsImplementedInterfaces();
builder.RegisterAssemblyTypes(typeof(UnitOfWorkFactory).Assembly).AsImplementedInterfaces();
builder.RegisterGeneric(typeof(CreateOrUpdateService<>)).As(typeof(ICreateOrUpdateService<>));
builder.RegisterGeneric(typeof(DetailService<>)).As(typeof(IDetailService<>));
builder.RegisterGeneric(typeof(ListService<>)).As(typeof(IListService<>));
builder.RegisterGeneric(typeof(DeleteService<>)).As(typeof(IDeleteService<>));
builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
//...
```

## Tech

DTORepository relies on these 2 open source projects to work properly:

* [AutoMapper] - Mapping Engine makes life easier
* [EntityFramework] - ORM framework for ADO.NET

This library is inspired by
* [GenericServices] -  a .NET class library to help build a service layer  by [JonPSmith]



License
----

MIT


   [AutoMapper]: <http://automapper.org/>
   [EntityFramework]: <https://github.com/aspnet/EntityFramework>
   [GenericServices]: <https://github.com/JonPSmith/GenericServices>
   [JonPSmith]: <https://github.com/JonPSmith>