using System.Collections.Generic;
using System.Reflection;
using Xunit;
using ProjectBank.Infrastructure.Entities;
using ProjectBank.Infrastructure;

namespace Seed.Tests;
public class SeedingTests
{
    [Fact]
    public void Institutions()
    {
        //Arrange
        List<Institution> Expected = new List<Institution>(){
            new Institution("ITU", "IT-Universitetet i København"),
            new Institution("KU", "Københavns Universitet")
        };
        //Act
        List<Institution> Actual = Seeding.Institutions;

        //Assert
        for (int i = 0; i < Expected.Count; i++)
        {
            Assert.Equal(Expected[i].Title, Actual[i].Title);
            Assert.Equal(Expected[i].Description, Actual[i].Description);
        }
        Assert.Equal(Expected.Count, Actual.Count);
    }

    [Fact]
    public void Faculties()
    {
        //Arrange
        var Institutions = Seeding.Institutions;
        var Expected = new List<Faculty>(){
            new Faculty("Computer Science", "Computers and Science", Institutions[0]),
            new Faculty("Digital Business","Business and digital stuff", Institutions[0]),
            new Faculty("Digital Design","Design and its digital!", Institutions[0]),
            new Faculty("Computer Science","Computers and Science", Institutions[1]),
            new Faculty("Medical Science","Medical science is important", Institutions[1]),
            new Faculty("Millitary Science","The mission is to educate the people on the most acclaimed millitary power in the world.", Institutions[1]),
            new Faculty("History", "Past time is old times", Institutions[1])
        };

        //Act
        var Actual = Seeding.Faculties;

        //Assert
        for (int i = 0; i < Expected.Count; i++)
        {
            Assert.Equal(Expected[i].Institution.Id, Actual[i].Institution.Id);
            Assert.Equal(Expected[i].Title, Actual[i].Title);
            Assert.Equal(Expected[i].Description, Actual[i].Description);
        }
        Assert.Equal(Expected.Count, Actual.Count);
    }

    [Fact]
    public void TeachingPrograms()
    {
        //Arrange
        var Institutions = Seeding.Institutions;
        var Faculties = Seeding.Faculties;
        var Expected = new List<TeachingProgram>(){
            new TeachingProgram("Software Development", "The development of software", Faculties[0], "SWU2021", new List<Course>()),
            new TeachingProgram("Data Science", "The science of data", Faculties[0],"DATA2021",new List<Course>()),
            new TeachingProgram("Global Business Informatics","The Link between businesses and software", Faculties[1],"GBI2021",new List<Course>()),
            new TeachingProgram("Digital Design and Interactive Technologies","Teachings in the digital and interactive field", Faculties[2],"DDS2021",new List<Course>()),
            new TeachingProgram("Software Development", "The development of software", Faculties[3], "SWU2021", new List<Course>()),
            new TeachingProgram("Ancient Greek Studies", "Study of the ancient Greeks", Faculties[6], "GRK2019", new List<Course>()),
            new TeachingProgram("Military Logisics", "The logistics of warfare", Faculties[5], "WAR2021", new List<Course>()),
            new TeachingProgram("Medicine", "Study of medicin and the human body", Faculties[4], "WAR2021", new List<Course>())
        };

        //Act
        var Actual = Seeding.TeachingPrograms;

        //Assert
        for (int i = 0; i < Expected.Count; i++)
        {
            Assert.Equal(Expected[i].Faculty.Title, Actual[i].Faculty.Title);
            Assert.Equal(Expected[i].Title, Actual[i].Title);
            Assert.Equal(Expected[i].Description, Actual[i].Description);
        }
        Assert.Equal(Expected.Count, Actual.Count);
    }
    [Fact]
    public void Supervisors()
    {
        //Arrange
        var Faculties = Seeding.Faculties;

        var ExpectedFirst = new Supervisor("annenielsen@itu.dk", Faculties[0].Institution, "Anne", "Nielsen", new List<Project>(), Faculties[0], new List<Project>());
        var ExpectedLast = new Supervisor("olepedersen@ku.dk", Faculties[3].Institution, "Ole", "Pedersen", new List<Project>(), Faculties[3], new List<Project>());

        //Act
        var Supervisors = Seeding.Supervisors;
        var ActualFirst = Supervisors[0];
        var ActualLast = Supervisors[Supervisors.Count -1];

        //Assert
        Assert.Equal(ExpectedFirst.Email, ActualFirst.Email);
        Assert.Equal(ExpectedFirst.Institution.Title, ActualFirst.Institution.Title);
        Assert.Equal(ExpectedFirst.FirstName, ActualFirst.FirstName);
        Assert.Equal(ExpectedFirst.LastName, ActualFirst.LastName);
        Assert.Equal(ExpectedFirst.Faculty.Title, ActualFirst.Faculty.Title);
        Assert.Equal(ExpectedLast.Email, ActualLast.Email);
        Assert.Equal(ExpectedLast.Institution.Title, ActualLast.Institution.Title);
        Assert.Equal(ExpectedLast.FirstName, ActualLast.FirstName);
        Assert.Equal(ExpectedLast.LastName, ActualLast.LastName);
        Assert.Equal(ExpectedLast.Faculty.Title, ActualLast.Faculty.Title);
    }

    [Fact]
    public void GenerateStudents_generates_students()
    {
        //Arrange
        var TeachingPrograms = Seeding.TeachingPrograms;

        var ExpectedFirst = new Student("annechristensen@itu.dk", TeachingPrograms[0].Faculty.Institution, "Anne", "Christensen", new List<Project>(), TeachingPrograms[0], new List<Course>());
        var ExpectedLast = new Student("olemortensen@ku.dk", TeachingPrograms[7].Faculty.Institution, "Ole", "Mortensen", new List<Project>(), TeachingPrograms[7], new List<Course>());
        
        //Act
        var Students = Seeding.Students;
        var ActualFirst = Students[0];
        var ActualLast = Students[Students.Count -1];

        //Assert
        Assert.Equal(ExpectedFirst.Email, ActualFirst.Email);
        Assert.Equal(ExpectedFirst.Institution.Title, ActualFirst.Institution.Title);
        Assert.Equal(ExpectedFirst.FirstName, ActualFirst.FirstName);
        Assert.Equal(ExpectedFirst.LastName, ActualFirst.LastName);
        Assert.Equal(ExpectedFirst.Program.Title, ActualFirst.Program.Title);
        Assert.Equal(ExpectedLast.Email, ActualLast.Email);
        Assert.Equal(ExpectedLast.Institution.Title, ActualLast.Institution.Title);
        Assert.Equal(ExpectedLast.FirstName, ActualLast.FirstName);
        Assert.Equal(ExpectedLast.LastName, ActualLast.LastName);
        Assert.Equal(ExpectedLast.Program.Title, ActualLast.Program.Title);
    }

    [Fact]
    public void GenerateTags()
    {
        //Arrange
        var Expected = new List<Tag>(){
        new Tag("Language"),
        new Tag("Mathematics"),
        new Tag("Science"),
        new Tag("Health"),
        new Tag("Fitness"),
        new Tag("Art"),
        new Tag("Music"),
        new Tag("Movement"),
        new Tag("Eurythmy"),
        new Tag("Gardening"),
        new Tag("Plants"),
        new Tag("Dramatics"),
        new Tag("Dance"),
        new Tag("Spanish"),
        new Tag("Leadership"),
        new Tag("Speech"),
        new Tag("Cartography"),
        new Tag("Encryption"),
        new Tag("Navigation"),
        new Tag("Programming"),
        new Tag("Math"),
        new Tag("Literature"),
        new Tag("Philosophy"),
        new Tag("Algorithms"),
        new Tag("Java"),
        new Tag("Dotnet"),
        new Tag("Network"),
        new Tag("Heuristic"),
        new Tag("UML"),
        new Tag("Docker"),
        new Tag("C#"),
        new Tag("Golang"),
        new Tag("Python"),
        new Tag("Warfare"),
        new Tag("Technology"),
        new Tag("Economics"),
        new Tag("East Asia"),
        new Tag("USA"),
        new Tag("Surveillance"),
        new Tag("Logistics"),
        new Tag("Military"),
        new Tag("Agriculture"),
        new Tag("Mink")
    };
        
        //Act
        var Actual = Seeding.Tags;

        //Assert
        Assert.Equal(Expected.Count, Actual.Count);
        for(int i = 0; i < Expected.Count; i++)
        {
            Assert.Equal(Expected[i].Name, Actual[i].Name);
        }
    }

    [Fact]
    public void GenerateProjects()
    {
        //Arrange
        var Expected = 2000;
        
        //Act
        List<Project> Projects = Seeding.Projects;
        var Actual = Projects.Count;

        //Assert
        Assert.Equal(Expected, Actual);

    }
}