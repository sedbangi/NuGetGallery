﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Moq;
using NuGet.Services.Entities;
using NuGet.Services.FeatureFlags;
using Xunit;

namespace NuGetGallery
{
    public class FeatureFlagServiceFacts
    {
        public class TheIsManageDeprecationEnabledMethod
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void WhenManageDeprecationFeatureIsDisabled_ReturnsFalse(bool hasManyVersions)
            {
                // Arrange
                var user = new User();
                var registration = new PackageRegistration();
                if (hasManyVersions)
                {
                    for (var i = 0; i < 1000; i++)
                    {
                        registration.Packages.Add(new Package { Key = i });
                    }
                }

                var clientMock = new Mock<IFeatureFlagClient>();
                clientMock
                    .Setup(c => c.IsEnabled("NuGetGallery.ManageDeprecation", It.IsAny<IFlightUser>(), false))
                    .Returns(false);

                var service = new FeatureFlagService(clientMock.Object);

                // Act
                var result = service.IsManageDeprecationEnabled(user, registration);

                // Assert
                Assert.False(result);
            }

            [Fact]
            public void WhenManageDeprecationFeatureIsEnabledAndFewVersions_ReturnsTrue()
            {
                // Arrange
                var user = new User();
                var registration = new PackageRegistration();

                var clientMock = new Mock<IFeatureFlagClient>();
                clientMock
                    .Setup(c => c.IsEnabled("NuGetGallery.ManageDeprecation", It.IsAny<IFlightUser>(), false))
                    .Returns(true);

                var service = new FeatureFlagService(clientMock.Object);

                // Act
                var result = service.IsManageDeprecationEnabled(user, registration);

                // Assert
                Assert.True(result);
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void WhenManageDeprecationFeatureIsEnabledAndManyVersions_ReturnsIfManageDeprecationForManyVersionsIsEnabled(
                bool isManyVersionsEnabled)
            {
                // Arrange
                var user = new User();
                var registration = new PackageRegistration();
                for (var i = 0; i < 1000; i++)
                {
                    registration.Packages.Add(new Package { Key = i });
                }

                var clientMock = new Mock<IFeatureFlagClient>();
                clientMock
                    .Setup(c => c.IsEnabled("NuGetGallery.ManageDeprecation", It.IsAny<IFlightUser>(), false))
                    .Returns(true);

                clientMock
                    .Setup(c => c.IsEnabled("NuGetGallery.ManageDeprecationMany", It.IsAny<IFlightUser>(), true))
                    .Returns(isManyVersionsEnabled);

                var service = new FeatureFlagService(clientMock.Object);

                // Act
                var result = service.IsManageDeprecationEnabled(user, registration);

                // Assert
                Assert.Equal(isManyVersionsEnabled, result);
            }
        }
    }
}
