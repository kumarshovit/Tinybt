using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.SharedKernel;
using NSubstitute;
using Shouldly;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Specifications;
using TinyBtUrlApi.UseCases.Account.Register;
using Xunit;

namespace TinyBtUrlApi.UnitTests.UseCases.Account.Register;

public class RegisterHandlerTests
{
    private readonly IRepository<User> _userRepository = Substitute.For<IRepository<User>>();
    private readonly IEmailSender _emailSender = Substitute.For<IEmailSender>();
    private readonly IEmailValidationService _emailValidationService = Substitute.For<IEmailValidationService>();
    private readonly string _frontendUrl = "https://link.bt";
    private readonly string _smtpFrom = "noreply@link.bt";

    private readonly RegisterHandler _handler;

    public RegisterHandlerTests()
    {
        _handler = new RegisterHandler(
            _userRepository,
            _emailSender,
            _emailValidationService,
            _frontendUrl,
            _smtpFrom);
    }

    [Fact]
    public async Task Handle_ValidNonDisposableEmail_ReturnsSuccess_PersistsUser_AndSendsVerificationEmail()
    {
        // Arrange
        var email = "john.doe@legitcompany.com";
        var query = new RegisterQuery(new RegisterDto
        {
            Email = email,
            Password = "SecurePassword123!"
        });

        _userRepository.FirstOrDefaultAsync(Arg.Any<UserByEmailSpec>())
            .Returns((User?)null);

        _emailValidationService.ValidateEmailAsync(email, Arg.Any<CancellationToken>())
            .Returns(EmailValidationResult.CreateSuccess());

        // Act
        var result = await _handler.Handle(query);

        // Assert
        result.Success.ShouldBeTrue();
        result.Message.ShouldContain("Registration successful");

        await _userRepository.Received(1).AddAsync(Arg.Is<User>(u =>
            u.Email == email.ToLower() &&
            !u.IsEmailVerified &&
            !string.IsNullOrEmpty(u.EmailVerificationToken)));

        await _emailSender.Received(1).SendEmailAsync(
            email.ToLower(),
            _smtpFrom,
            "Verify Your Email",
            Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_ConfirmedDisposableEmail_ReturnsFailure_DoesNotPersistUser_AndDoesNotSendEmail()
    {
        // Arrange
        var email = "spammer@10minutemail.com";
        var query = new RegisterQuery(new RegisterDto
        {
            Email = email,
            Password = "SecurePassword123!"
        });

        _userRepository.FirstOrDefaultAsync(Arg.Any<UserByEmailSpec>())
            .Returns((User?)null);

        _emailValidationService.ValidateEmailAsync(email, Arg.Any<CancellationToken>())
            .Returns(EmailValidationResult.CreateDisposable());

        // Act
        var result = await _handler.Handle(query);

        // Assert
        result.Success.ShouldBeFalse();
        result.Message.ShouldContain("Disposable or temporary email addresses are not permitted.");

        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>());
        await _emailSender.DidNotReceive().SendEmailAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_InvalidFormatFromAbstract_ReturnsFailure_DoesNotPersistUser()
    {
        // Arrange
        var email = "badformat@domain.xyz";
        var query = new RegisterQuery(new RegisterDto
        {
            Email = email,
            Password = "SecurePassword123!"
        });

        _userRepository.FirstOrDefaultAsync(Arg.Any<UserByEmailSpec>())
            .Returns((User?)null);

        _emailValidationService.ValidateEmailAsync(email, Arg.Any<CancellationToken>())
            .Returns(EmailValidationResult.CreateInvalidFormat("Invalid email format."));

        // Act
        var result = await _handler.Handle(query);

        // Assert
        result.Success.ShouldBeFalse();
        result.Message.ShouldBe("Invalid email format.");
        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_UndeliverableStatus_ReturnsFailure_DoesNotPersistUser()
    {
        // Arrange
        var email = "nonexistent@realdomain.com";
        var query = new RegisterQuery(new RegisterDto
        {
            Email = email,
            Password = "SecurePassword123!"
        });

        _userRepository.FirstOrDefaultAsync(Arg.Any<UserByEmailSpec>())
            .Returns((User?)null);

        _emailValidationService.ValidateEmailAsync(email, Arg.Any<CancellationToken>())
            .Returns(EmailValidationResult.CreateUndeliverable("Email address is undeliverable."));

        // Act
        var result = await _handler.Handle(query);

        // Assert
        result.Success.ShouldBeFalse();
        result.Message.ShouldBe("Email address is undeliverable.");
        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_DuplicateVerifiedEmail_ReturnsFailure_DoesNotCallValidationService()
    {
        // Arrange
        var email = "existing@example.com";
        var query = new RegisterQuery(new RegisterDto
        {
            Email = email,
            Password = "SecurePassword123!"
        });

        var existingUser = new User
        {
            Email = email,
            IsEmailVerified = true
        };

        _userRepository.FirstOrDefaultAsync(Arg.Any<UserByEmailSpec>())
            .Returns(existingUser);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        result.Success.ShouldBeFalse();
        result.Message.ShouldBe("Email already exists.");

        // Quota protection: must NOT call external validation for existing verified account
        await _emailValidationService.DidNotReceive().ValidateEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_DuplicateUnverifiedEmail_ResendsVerificationEmail_PreservesExistingBehavior()
    {
        // Arrange
        var email = "unverified@example.com";
        var query = new RegisterQuery(new RegisterDto
        {
            Email = email,
            Password = "SecurePassword123!"
        });

        var existingUser = new User
        {
            Email = email,
            IsEmailVerified = false,
            EmailVerificationToken = "old-token"
        };

        _userRepository.FirstOrDefaultAsync(Arg.Any<UserByEmailSpec>())
            .Returns(existingUser);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        result.Success.ShouldBeTrue();
        result.Message.ShouldContain("Verification email resent");

        existingUser.EmailVerificationToken.ShouldNotBe("old-token");
        await _userRepository.Received(1).UpdateAsync(existingUser);
        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>());

        // Resend path does not consume external validation credits
        await _emailValidationService.DidNotReceive().ValidateEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ProviderUnavailable_FollowsFailClosedPolicy_ReturnsTemporaryFailure()
    {
        // Arrange
        var email = "newuser@example.com";
        var query = new RegisterQuery(new RegisterDto
        {
            Email = email,
            Password = "SecurePassword123!"
        });

        _userRepository.FirstOrDefaultAsync(Arg.Any<UserByEmailSpec>())
            .Returns((User?)null);

        _emailValidationService.ValidateEmailAsync(email, Arg.Any<CancellationToken>())
            .Returns(EmailValidationResult.CreateProviderUnavailable());

        // Act
        var result = await _handler.Handle(query);

        // Assert
        result.Success.ShouldBeFalse();
        result.Message.ShouldContain("We're temporarily unable to verify this email address. Please try again shortly.");

        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>());
        await _emailSender.DidNotReceive().SendEmailAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_RegexFormatValidationFailure_RejectsBeforeCallingValidationService()
    {
        // Arrange
        var query = new RegisterQuery(new RegisterDto
        {
            Email = "invalid-plain-text",
            Password = "SecurePassword123!"
        });

        // Act
        var result = await _handler.Handle(query);

        // Assert
        result.Success.ShouldBeFalse();
        result.Message.ShouldBe("Invalid email format.");

        await _emailValidationService.DidNotReceive().ValidateEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _userRepository.DidNotReceive().FirstOrDefaultAsync(Arg.Any<UserByEmailSpec>());
    }

    [Fact]
    public async Task Handle_WeakPassword_RejectsBeforeCallingValidationService()
    {
        // Arrange
        var query = new RegisterQuery(new RegisterDto
        {
            Email = "valid@example.com",
            Password = "weak"
        });

        // Act
        var result = await _handler.Handle(query);

        // Assert
        result.Success.ShouldBeFalse();
        result.Message.ShouldContain("Password must be at least 8 characters");

        await _emailValidationService.DidNotReceive().ValidateEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_GmailWithDotsAndPlus_NormalizesAndSavesCanonicalEmail()
    {
        // Arrange
        var inputEmail = "ta63266.0+promo@gmail.com";
        var expectedCanonical = "ta632660@gmail.com";
        var query = new RegisterQuery(new RegisterDto
        {
            Email = inputEmail,
            Password = "SecurePassword123!"
        });

        _userRepository.FirstOrDefaultAsync(Arg.Any<UserByEmailSpec>())
            .Returns((User?)null);

        _emailValidationService.ValidateEmailAsync(inputEmail, Arg.Any<CancellationToken>())
            .Returns(EmailValidationResult.CreateSuccess());

        // Act
        var result = await _handler.Handle(query);

        // Assert
        result.Success.ShouldBeTrue();
        result.Message.ShouldContain("Registration successful");

        await _userRepository.Received(1).AddAsync(Arg.Is<User>(u =>
            u.Email == expectedCanonical &&
            !u.IsEmailVerified &&
            u.EmailVerificationToken != null &&
            u.EmailVerificationToken.Contains("."))); // verify timestamped token format
    }

    [Fact]
    public async Task Handle_GmailDotVariationWhenCanonicalAlreadyExists_RejectsAsDuplicate()
    {
        // Arrange: canonical ta632660@gmail.com already exists in DB
        var existingCanonical = new User
        {
            Email = "ta632660@gmail.com",
            IsEmailVerified = true
        };

        _userRepository.FirstOrDefaultAsync(Arg.Is<UserByEmailSpec>(spec => true))
            .Returns(existingCanonical);

        // Act: try to register with a dot variation ta63266.0@gmail.com
        var query = new RegisterQuery(new RegisterDto
        {
            Email = "ta63266.0@gmail.com",
            Password = "SecurePassword123!"
        });

        var result = await _handler.Handle(query);

        // Assert
        result.Success.ShouldBeFalse();
        result.Message.ShouldBe("Email already exists.");

        // Quota protection: must NOT call external validation
        await _emailValidationService.DidNotReceive().ValidateEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_CaptchaValidationFails_RejectsRegistrationBeforeValidationOrDb()
    {
        // Arrange: handler configured with ICaptchaService mock
        var captchaService = Substitute.For<ICaptchaService>();
        captchaService.VerifyTokenAsync(Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var handlerWithCaptcha = new RegisterHandler(
            _userRepository,
            _emailSender,
            _emailValidationService,
            _frontendUrl,
            _smtpFrom,
            captchaService);

        var query = new RegisterQuery(new RegisterDto
        {
            Email = "test@example.com",
            Password = "SecurePassword123!",
            CaptchaToken = "invalid-token"
        });

        // Act
        var result = await handlerWithCaptcha.Handle(query);

        // Assert
        result.Success.ShouldBeFalse();
        result.Message.ShouldBe("CAPTCHA verification failed.");

        await _emailValidationService.DidNotReceive().ValidateEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>());
    }
}
