using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using AutoUsing.Analysis;
using AutoUsing.Analysis.Cache;
using AutoUsing.DataTypes;
using AutoUsing.Models;
using FireSharp.Extensions;


namespace AutoUsing
{
    /// <summary>
    /// Specifies the logic for every request
    /// </summary>
    public class Server
    {
        public IOProxy Proxy = new IOProxy();
        private List<Project> Projects = new List<Project>();

        public Response Pong(Request req)
        {
            return new SuccessResponse {Body = "pong"};
        }

        public Response Error(string error)
        {
            return new ErrorResponse {Body = error};
        }

        public void Listen()
        {
            while (true)
            {
                Proxy.ReadData(new MessageEventArgs {Data = Console.ReadLine()});
            }
        }


        public Response GetAllReferences(GetAllReferencesRequest req)
        {
            var projectName = req.ProjectName;

            if (projectName.IsNullOrEmpty())
            {
                return new ErrorResponse {Body = Errors.ProjectNameIsRequired};
            }

            // Using C# 7.2 `is expression` to check for null, and assign variable
            if (Projects.Find(o => o.Name == projectName) is Project project)
            {
                var referenceInfo = GlobalCache.Caches.Types.Get();
                referenceInfo.AddRange(project.Caches.Types.Get());
                return new GetAllReferencesResponse
                {
                    References = CompletionCaches.ToCompletionFormat(referenceInfo)
                        .Where(reference => reference.Name.StartsWith(req.WordToComplete)).ToList()
                };
            }
            else
            {
                return new ErrorResponse {Body = Errors.SpecifiedProjectWasNotFound};
            }
        }

        //TODO
        public void AddProject(Request req)
        {
            // var projectFilePath = req.Arguments;

            // if (!projectFilePath.IsNullOrEmpty())
            // {
            //     Projects.Add(new Project(projectFilePath, watch: true));
            //     return;
            // }

            // Proxy.WriteData(new ErrorResponse { Body = Errors.ProjectFilePathIsRequired });
        }

        //TODO
        public void RemoveProject(Request req)
        {
            // var projectName = req.Arguments;

            // if (!projectName.IsNullOrEmpty())
            // {
            //     // One line torture :D
            //     foreach (var project in Projects.Select(o => { if (o.Name != projectName) return null; o.Dispose(); return o; }))
            //     {
            //         Projects.Remove(project);
            //     }

            //     return;
            // }

            // Proxy.WriteData(new ErrorResponse { Body = Errors.ProjectNameIsRequired });
        }

        public void AddCmdArgProjects(string[] projects)
        {
            foreach (var path in projects)
            {
                Projects.Add(new Project(path, watch: true));
            }
        }

        public Response AddProjects(AddProjectsRequest req)
        {
            if (req.Projects.Any(path => !File.Exists(path)))
            {
                return new ErrorResponse {Body = Errors.NonExistentProject};
            }

            const string csproj = ".csproj";

            if (req.Projects.Any(path => Path.GetExtension(path) != csproj))
            {
                return new ErrorResponse {Body = Errors.NonExistentProject};
            }

            Projects.AddRange(req.Projects.Select(path => new Project(path, watch: true)));
            return new SuccessResponse();
        }
    }
}