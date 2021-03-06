﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using MedApp.Core;
using MedApp.Core.Models;
using MedApp.Core.Repositories;
using NUnit.Framework;

namespace MedApp.BLL.Tests
{
    [TestFixture]
    public class CreatePatientTests
    {
        private static (Mock<IUnitOfWork> unitOfWork, Mock<IPatientRepository> patientRepo, Dictionary<int, Patient> dbCollection) GetMocks()
        {
            var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
            var patientRepo = new Mock<IPatientRepository>(MockBehavior.Strict);
            var dbCollection = new Dictionary<int, Patient>
            {
                [26] = new Patient
                {
                    Id = 26,
                    FullName = "Delete Name"
                },
                [27] = new Patient
                {
                    Id = 27,
                    FullName = "Name"
                }
            };

            unitOfWork.SetupGet(e => e.Patients).Returns(patientRepo.Object);
            unitOfWork.Setup(e => e.CommitAsync()).ReturnsAsync(0);

            patientRepo.Setup(e => e.AddAsync(It.IsAny<Patient>()))
                      .Callback((Patient newPatient) => { dbCollection.Add(newPatient.Id, newPatient); })
                      .Returns((Patient _) => Task.CompletedTask);

            return (unitOfWork, patientRepo, dbCollection);
        }

        [Test]
        public async Task CreatePatient_FullInfo_Success()
        {
            // Arrange
            var (unitOfWork, patientRepo, dbCollection) = GetMocks();
            var service = new PatientService(unitOfWork.Object);
            var patient = new Patient
            {
                Id = 28,
                FullName = "New Name"
            };

            // Act
            await service.CreatePatient(patient);

            // Assert
            Assert.IsTrue(dbCollection.ContainsKey(patient.Id));
        }

        [Test]
        public void CreatePatient_NullObject_NullReferenceException()
        {
            // Arrange
            var (unitOfWork, patientRepo, dbCollection) = GetMocks();
            var service = new PatientService(unitOfWork.Object);

            // Act + Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await service.CreatePatient(null));
        }
    }
}