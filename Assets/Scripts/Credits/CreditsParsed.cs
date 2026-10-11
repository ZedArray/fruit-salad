using System.Collections.Generic;

public class Category
{
    public string title { get; set; }
    public List<Person> people { get; set; }
}

public class Person
{
    public string name { get; set; }
    public string alias { get; set; }
    public List<string> links { get; set; }
    public List<string> additional { get; set; }
}

public class CreditsParsed
{
    public List<Category> categories { get; set; }
}
