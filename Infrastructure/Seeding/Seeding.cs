using System.Text;
using ProjectBank.Infrastructure.Entities;

namespace ProjectBank.Infrastructure;
public class Seeding
{
    private static readonly List<string> FirstNames = new List<string>(){
        "Anne",
        "Kirsten",
        "Mette",
        "Hanne",
        "Helle",
        "Anna",
        "Susanne",
        "Lene",
        "Maria",
        "Marianne",
        "Lone",
        "Camilla",
        "Pia",
        "Louise",
        "Charlotte",
        "Tina",
        "Gitte",
        "Bente",
        "Jette",
        "Karen",
        "Peter",
        "Michael",
        "Jens",
        "Lars",
        "Thomas",
        "Henrik",
        "Søren",
        "Christian",
        "Jan",
        "Martin",
        "Niels",
        "Anders",
        "Morten",
        "Jesper",
        "Mads",
        "Rasmus",
        "Hans",
        "Per",
        "Jørgen",
        "Ole"
    };

    private static readonly List<string> LastNames = new List<string>(){
       "Nielsen",
       "Jensen",
       "Hansen",
       "Andersen",
       "Pedersen",
       "Christensen",
       "Larsen",
       "Sørensen",
       "Rasmussen",
       "Jørgensen",
       "Petersen",
       "Madsen",
       "Kristensen",
       "Olsen",
       "Thomsen",
       "Christiansen",
       "Poulsen",
       "Møller",
       "Slette-Frederiksen",
       "Mortensen"
    };

    public static readonly List<Tag> Tags = new List<Tag>(){
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


    public static readonly List<Institution> Institutions =
        new List<Institution>(){
            new Institution("ITU", "IT-Universitetet i København"),
            new Institution("KU", "Københavns Universitet")
        };

    public static readonly IList<Faculty> Faculties = new List<Faculty>(){
        new Faculty("Computer Science", "Computers and Science", Institutions[0]),
        new Faculty("Digital Business","Business and digital stuff", Institutions[0]),
        new Faculty("Digital Design","Design and its digital!", Institutions[0]),
        new Faculty("Computer Science","Computers and Science", Institutions[1]),
        new Faculty("Medical Science","Medical science is important", Institutions[1]),
        new Faculty("Millitary Science","The mission is to educate the people on the most acclaimed millitary power in the world.", Institutions[1]),
        new Faculty("History", "Past time is old times", Institutions[1])
    };

    public static readonly List<TeachingProgram> TeachingPrograms = new List<TeachingProgram>(){
            new TeachingProgram("Software Development", "The development of software", Faculties[0], "SWU2021", new List<Course>()),
            new TeachingProgram("Data Science", "The science of data", Faculties[0],"DATA2021",new List<Course>()),
            new TeachingProgram("Global Business Informatics","The Link between businesses and software", Faculties[1],"GBI2021",new List<Course>()),
            new TeachingProgram("Digital Design and Interactive Technologies","Teachings in the digital and interactive field", Faculties[2],"DDS2021",new List<Course>()),
            new TeachingProgram("Software Development", "The development of software", Faculties[3], "SWU2021", new List<Course>()),
            new TeachingProgram("Ancient Greek Studies", "Study of the ancient Greeks", Faculties[6], "GRK2019", new List<Course>()),
            new TeachingProgram("Military Logisics", "The logistics of warfare", Faculties[5], "WAR2021", new List<Course>()),
            new TeachingProgram("Medicine", "Study of medicin and the human body", Faculties[4], "WAR2021", new List<Course>())
        };

    public static readonly List<Supervisor> Supervisors = GenerateSupervisors();

    private static List<Supervisor> GenerateSupervisors()
    {
        List<Supervisor> newSupervisors = new List<Supervisor>();
        int facultyIndex = 0;

        for (int i = 0; i < FirstNames.Count * LastNames.Count / 4; i++, facultyIndex++)
        {
            string FirstName = FirstNames[i % FirstNames.Count];
            string LastName = LastNames[i / FirstNames.Count];
            facultyIndex = facultyIndex % Faculties.Count;

            StringBuilder email = new StringBuilder();
            email.Append(FirstName.ToLower());
            email.Append(LastName.ToLower());
            email.Append(string.Format("@{0}.dk", Faculties[facultyIndex].Institution.Title.ToLower()));
            newSupervisors.Add(new Supervisor(email.ToString(), Faculties[facultyIndex].Institution, FirstName, LastName, new List<Project>(), Faculties[facultyIndex], new List<Project>()));
        }
        return newSupervisors;
    }

    public static readonly List<Student> Students = GenerateStudents();

    private static List<Student> GenerateStudents()
    {
        List<Student> Students = new List<Student>();
        int TeachingProgramIndex = 0;

        for (int i = FirstNames.Count * LastNames.Count / 4; i < FirstNames.Count * LastNames.Count; i++, TeachingProgramIndex++)
        {
            string FirstName = FirstNames[i % FirstNames.Count];
            string LastName = LastNames[i / FirstNames.Count];
            TeachingProgramIndex %= TeachingPrograms.Count;

            StringBuilder email = new StringBuilder();
            email.Append(FirstName.ToLower());
            email.Append(LastName.ToLower());
            email.Append(string.Format("@{0}.dk", TeachingPrograms[TeachingProgramIndex].Faculty.Institution.Title.ToLower()));
            Students.Add(new Student(email.ToString(), TeachingPrograms[TeachingProgramIndex].Faculty.Institution, FirstName, LastName, new List<Project>(), TeachingPrograms[TeachingProgramIndex], new List<Course>()));
        }
        return Students;
    }

    public static readonly List<Project> Projects = GenerateProjects(10, 10, 10);
    private static List<Project> GenerateProjects(int ProjectsPerSupervisor, int TagsPerProject, int StudentsPerProject)
    {
        Random random = new Random();
        if(TagsPerProject >= Tags.Count) throw new Exception(string.Format("Too many tags per project. There are only {0} tags in total",Tags.Count));
        List<Project> Projects = new List<Project>();
        int ProjectNum = 1;
        
        foreach(Supervisor supervisor in Supervisors)
        {
            List<Student> EligibleStudents = Students.Where(x => x.Program.Faculty.Id == supervisor.Faculty.Id).ToList();

            for(int i = 0; i < ProjectsPerSupervisor; i++)
            {
                HashSet<Tag> TagsInProject = new HashSet<Tag>();
                while(TagsInProject.Count < TagsPerProject) TagsInProject.Add(Tags[random.Next(0, Tags.Count - 1)]);
                List<Tag>TagsInProjectList = TagsInProject.ToList();
                
                HashSet<User> Users = new HashSet<User>();
                while(Users.Count < StudentsPerProject) Users.Add(EligibleStudents.ElementAt(random.Next(0, EligibleStudents.Count - 1)));

                StringBuilder Title = new StringBuilder();
                Title.Append(string.Format("Project about {0} and {1}", TagsInProjectList[0].Name, TagsInProjectList[1].Name));
                Title.Append(string.Format(" {0}", ProjectNum));

                StringBuilder Description = new StringBuilder();
                Description.Append("This project is about");
                for(int t = 0; t < TagsInProjectList.Count; t++){
                    string append;
                    if(t == 0) append = string.Format(" {0}", TagsInProjectList[t].Name);
                    else if (t == TagsInProjectList.Count - 1) append = string.Format(" and {0}.", TagsInProjectList[t].Name);
                    else append = string.Format(", {0}",TagsInProjectList[t].Name);
                    Description.Append(append);
                }

                Random rnd = new Random();
                var randomStatus = rnd.Next(1,5);
                var status = new ProjectStatus();

                if (randomStatus == 1)
                {
                    status = ProjectStatus.PUBLIC;

                } else if (randomStatus == 2)
                {
                    status = ProjectStatus.PRIVATE;

                } else if (randomStatus == 3)
                {
                    status = ProjectStatus.DRAFT;
                    
                } else 
                {
                    status = ProjectStatus.DELETED;
                }
                
                Projects.Add(new Project(supervisor, Title.ToString(), Description.ToString(), status, supervisor.Faculty, TagsInProjectList.AsReadOnly(), Users, new List<ProjectBucket>(), StudentsPerProject));
                ProjectNum++;
            }
        }
        return Projects;
    }

}