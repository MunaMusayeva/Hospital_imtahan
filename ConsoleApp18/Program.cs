using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

class Department
{
    public string Name { get; set; }
    public List<Doctor> Doctors { get; set; }

    public Department(string name, List<Doctor> doctors)
    {
        Name = name;
        Doctors = doctors;
    }

    public void SaveToJson()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(this, options);
            File.WriteAllText($"{Name}_department.json", jsonString);
            Console.WriteLine($"{Name} sobesi JSON faylina saxlanildi.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Sobe melumatlarini JSON faylına yazarken xeta baş verdi: {ex.Message}");
        }
    }
}

class Doctor : Person
{
    public int Experience { get; set; }
    public string[] AppointmentTimes { get; set; }
    public bool[] Reserved { get; set; }

    public Doctor(string firstName, string lastName, int experience)
        : base(firstName, lastName)
    {
        Experience = experience;
        AppointmentTimes = new[] { "09:00-11:00", "12:00-14:00", "15:00-17:00" };
        Reserved = new bool[3];
    }

    public static void ReserveAppointment(User user, Doctor doctor)
    {
        while (true)
        {
            try
            {
                Console.WriteLine($"Zehmet olmasa saati secin ({doctor.FirstName} {doctor.LastName}):");
                for (int i = 0; i < doctor.AppointmentTimes.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {doctor.AppointmentTimes[i]} - {(doctor.Reserved[i] ? "rezerv olunub" : "rezerv olunmayıb")}");
                }

                int timeIndex = int.Parse(Console.ReadLine()) - 1;

                if (!doctor.Reserved[timeIndex])
                {
                    doctor.Reserved[timeIndex] = true;
                    Console.WriteLine($"Tesekkurler {user.FirstName} {user.LastName}, siz saat {doctor.AppointmentTimes[timeIndex]}-da {doctor.FirstName} hekimin qebuluna yazildiniz.");
                    break;
                }
                else
                {
                    Console.WriteLine("Hemin vaxt artiq rezerv olunub, zehmet olmasa basqa vaxt secin.");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Zehmət olmasa duzgun vaxt indeksini daxil edin.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Rezervasiya zamanı xeta bas verdi: {ex.Message}");
            }
        }
    }

    public static Doctor SelectDoctor(Department department)
    {
        try
        {
            Console.WriteLine($"Zehmet olmasa {department.Name} sobesinde bir hekim secin:");
            for (int i = 0; i < department.Doctors.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {department.Doctors[i].FirstName} {department.Doctors[i].LastName} ({department.Doctors[i].Experience} il tecrube)");
            }

            int doctorIndex = int.Parse(Console.ReadLine()) - 1;
            return department.Doctors[doctorIndex];
        }
        catch (FormatException)
        {
            Console.WriteLine("Zehmet olmasa duzgun hekim indeksini daxil edin.");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hekim seçimi zamani xeta baş verdi: {ex.Message}");
            return null;
        }
    }

    public void SaveToJson()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(this, options);
            File.WriteAllText($"{FirstName}_{LastName}_doctor.json", jsonString);
            Console.WriteLine($"{FirstName} {LastName} hekimi JSON faylina saxlanildi.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hekim melumatlarini JSON faylina yazarken xeta bas verdi: {ex.Message}");
        }
    }
}

abstract class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    protected Person(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}

class Program
{
    static List<Department> departments = new List<Department>
    {
        new Department("Pediatriya", new List<Doctor>
        {
            new Doctor("Ali", "Aliyev", 10),
            new Doctor("Veli", "Veliev", 5),
            new Doctor("Sara", "Quliyeva", 8)
        }),
        new Department("Travmatologiya", new List<Doctor>
        {
            new Doctor("Hasan", "Hasanov", 12),
            new Doctor("Nigar", "Mammadova", 7)
        }),
        new Department("Stamotologiya", new List<Doctor>
        {
            new Doctor("Leyla", "Huseynova", 15),
            new Doctor("Kamran", "Jafarov", 6),
            new Doctor("Narmin", "Qasimova", 9),
            new Doctor("Rasim", "Ibrahimov", 4)
        })
    };

    static List<User> users = new List<User>();

    static void Main(string[] args)
    {
        while (true)
        {
            try
            {
                User user = GetUserInformation();
                if (user != null)
                {
                    users.Add(user);
                    Department selectedDepartment = SelectDepartment();
                    if (selectedDepartment != null)
                    {
                        Doctor selectedDoctor = Doctor.SelectDoctor(selectedDepartment);
                        if (selectedDoctor != null)
                        {
                            Doctor.ReserveAppointment(user, selectedDoctor);
                            SaveDepartmentsToJson();
                            SaveUsersToJson();
                            selectedDoctor.SaveToJson();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Proqram icrasi zamani xeta bas verdi: {ex.Message}");
            }
        }
    }

    static User GetUserInformation()
    {
        try
        {
            Console.WriteLine("Adinizi daxil edin:");
            string firstName = Console.ReadLine();

            Console.WriteLine("Soyadinizi daxil edin:");
            string lastName = Console.ReadLine();

            Console.WriteLine("Emailinizi daxil edin:");
            string email = Console.ReadLine();

            Console.WriteLine("Telefon nomrenizi daxil edin:");
            string phone = Console.ReadLine();

            return new User(firstName, lastName, email, phone);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"İstifadeci melumatlarini daxil ederken xeta bas verdi: {ex.Message}");
            return null;
        }
    }

    static Department SelectDepartment()
    {
        try
        {
            Console.WriteLine("Zehmet olmasa sobeni secin:");
            for (int i = 0; i < departments.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {departments[i].Name}");
            }

            int departmentIndex = int.Parse(Console.ReadLine()) - 1;
            return departments[departmentIndex];
        }
        catch (FormatException)
        {
            Console.WriteLine("Zehmet olmasa duzgun sobe indeksini daxil edin.");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Sobe secimi zamanı xeta bas verdi: {ex.Message}");
            return null;
        }
    }

    static void SaveDepartmentsToJson()
    {
        try
        {
            foreach (var department in departments)
            {
                department.SaveToJson();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Sobeleri JSON faylına yazarken xeta bas verdi: {ex.Message}");
        }
    }

    static void SaveUsersToJson()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(users, options);
            File.WriteAllText("users.json", jsonString);
            Console.WriteLine("İstifadeçiler JSON faylına saxlanıldı.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"İstifadəçi məlumatlarını JSON faylına yazarkən xəta baş verdi: {ex.Message}");
        }
    }
}

class User : Person
{
    public string Email { get; set; }
    public string Phone { get; set; }

    public User(string firstName, string lastName, string email, string phone)
        : base(firstName, lastName)
    {
        Email = email;
        Phone = phone;
    }

    public void SaveToJson()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(this, options);
            File.WriteAllText($"{FirstName}_{LastName}_user.json", jsonString);
            Console.WriteLine($"{FirstName} {LastName} istifadecisi JSON faylına saxlanıldı.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"İstifadeçi məlumatlarını JSON faylına yazarken xeta baş verdi: {ex.Message}");
        }
    }
}
