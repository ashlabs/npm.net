﻿// -----------------------------------------------------------------------
// <copyright file="PackageManagerSample.cs" company="Microsoft">
// Sample usage of NpmPackageManager
// </copyright>
// -----------------------------------------------------------------------

namespace NpmApiSample
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using NodeNpm;

    /// <summary>
    /// A class that uses NpmPackageManager
    /// </summary>
    public static class PackageManagerSample
    {
        /// <summary>
        /// Exercise the NpmPackageManager class
        /// </summary>
        /// <param name="wd">working directory path</param>
        /// <param name="module">module name to use</param>
        /// <returns>true or false</returns>
        public static bool RunSample(string wd, string installPath, string module)
        {
            IEnumerable<string> uninstalled = null;

            NpmPackageManager npm = new NpmPackageManager(wd);
            if (npm == null)
            {
                Console.WriteLine("Failed to create NpmApi");
                return false;
            }

            npm.NpmClient.InstallPath = installPath;
            INpmSearchResultPackage found = null;
            IEnumerable<INpmSearchResultPackage> searchResults = npm.SearchRemotePackages(module);
            if (searchResults != null)
            {
                foreach (INpmSearchResultPackage result in searchResults)
                {
                    if (result.Name == module)
                    {
                        found = result;
                        break;
                    }
                }
            }

            if (found == null)
            {
                Console.WriteLine("SearchRemotePackages failed to find '{0}'", module);
                return false;
            }

            // install module as a dependency
            IEnumerable<INpmPackage> installed = npm.InstallPackage(found);
            if (installed == null || installed.Count() == 0)
            {
                Console.WriteLine("InstallPackage failed for {0}", found.Name);
                return false;
            }

            // list packages at parent
            IEnumerable<INpmInstalledPackage> installedPkg = npm.GetInstalledPackages();
            if (installedPkg == null)
            {
                Console.WriteLine("GetInstalledPackages failed for {0}", found.Name);
                return false;
            }

            // there should be at least 1 item since we installed one
            if (installedPkg.Count() == 0)
            {
                Console.WriteLine("There are no packages listed");
                return false;
            }

            // remove a dependency
            foreach (INpmInstalledPackage package in installedPkg)
            {
                if (package.DependentPath == found.Name)
                {
                    uninstalled = npm.UninstallPackage(package);
                    if (uninstalled == null)
                    {
                        Console.WriteLine("Failed to uninstall dependency {0} of {1}", package.Name, found.Name);
                        return false;
                    }

                    // Make sure IsPackageInstalled returns null
                    INpmInstalledPackage missingPackage = npm.IsPackageInstalled(package);
                    if (missingPackage != null)
                    {
                        Console.WriteLine("Test for installed after uninstall fails for {0} of {1}", package.Name, found.Name);
                        return false;
                    }

                    break;
                }
            }

            // check if dependency reported as missing
            IEnumerable<INpmPackageDependency> outdated = npm.FindDependenciesToBeInstalled(found);
            if (uninstalled != null)
            {
                if (outdated != null && outdated.Count() > 0)
                {
                    foreach (string uninstalledName in uninstalled)
                    {
                        bool matchMissing = false;
                        foreach (INpmPackageDependency outofdate in outdated)
                        {
                            if (outofdate.Name == uninstalledName &&
                                string.IsNullOrWhiteSpace(outofdate.Version))
                            {
                                matchMissing = true;
                                break;
                            }
                        }

                        if (!matchMissing)
                        {
                            Console.WriteLine("Test for outdated after uninstall fails for {0} of {1}", uninstalledName, found.Name);
                            return false;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Test for outdated after uninstall fails for {0}", found.Name);
                    return false;
                }
            }

            // now call update and check if it is fixed
            IEnumerable<string> updated = npm.UpdatePackage(found);
            if (updated == null)
            {
                Console.WriteLine("Update failed for {0}", found.Name);
                return false;
            }

            outdated = npm.FindDependenciesToBeInstalled(found);
            if (outdated != null && outdated.Count() > 0)
            {
                Console.WriteLine("Expected no outdated entry after update of {0}", module);
                return false;
            }

            Console.WriteLine("Success! {0} is installed.", module);
            return true;
        }
    }
}
