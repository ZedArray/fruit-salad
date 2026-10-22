using System;
using System.Collections.Generic;

// shape of Assets/Data/credits.json, read with JsonUtility (needs fields, not properties)
[Serializable]
public class Category
{
    public string title;
    public List<Person> people;
}

[Serializable]
public class Person
{
    public string name;
    public string alias;
    public List<string> links;
    public List<string> additional;
}

[Serializable]
public class CreditsParsed
{
    public List<Category> categories;
}
