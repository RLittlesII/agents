Feature: Host installation
  A consumer installs the complete method into a project for its selected
  host through Skillfile or Microsoft apm.

  @B-001
  Scenario: Install the complete Claude host
    Given a consumer project
    And a checkout with committed host projections
    When the installer is run for Claude
    Then every Claude role, method skill and command is installed in the project
    And no Codex host artifacts are installed

  @B-002
  Scenario: Install the complete Codex host
    Given a consumer project
    And a checkout with committed host projections
    When the installer is run for Codex
    Then every Codex role, method skill and command skill is installed in the project
    And no Claude host artifacts are installed

  @B-003
  Scenario Outline: Install through Skillfile
    Given a consumer project
    And Skillfile is available
    When the installer is run for <host> through Skillfile
    Then Skillfile is invoked
    And the selected host is completely installed

    Examples:
      | host   |
      | Claude |
      | Codex  |

  @B-004
  Scenario Outline: Install through Microsoft apm
    Given a consumer project
    And Microsoft apm is available
    When the installer is run for <host> through Microsoft apm
    Then Microsoft apm is invoked
    And the selected host is completely installed

    Examples:
      | host   |
      | Claude |
      | Codex  |

  @B-005
  Scenario Outline: Use the same interface across operating systems
    Given a consumer project on <operating system>
    And the documented installer prerequisites are available
    When the installer is run with the documented host and manager arguments
    Then the selected host is completely installed

    Examples:
      | operating system |
      | Windows          |
      | macOS            |
      | Linux            |

  @B-006
  Scenario Outline: Installed instructions retain their dependencies
    Given a complete installation for <host>
    When an installed role or command refers to a repository-owned contract or skill
    Then the referenced artifact exists at its installed location

    Examples:
      | host   |
      | Claude |
      | Codex  |

  @B-003 @B-004
  Scenario: The selected manager fails
    Given a consumer project
    And the selected manager returns a failure
    When the installer runs
    Then the installer exits unsuccessfully

  @boundary
  Scenario: Preserve consumer instructions
    Given a consumer project with its own host instructions and unrelated artifacts
    When a supported host is installed
    Then the consumer's instructions and unrelated artifacts are preserved

  @boundary
  Scenario: Reject an escaping installation link
    Given a consumer project with an installation path linked outside the project
    When the installer runs
    Then the installer exits unsuccessfully
    And no file outside the project is changed

  @B-007 @B-006
  Scenario Outline: Private sources contain runtime artifacts and a consumer index
    Given a checkout with maintainer documentation and nested specification metadata
    And selected runtime role contracts and method resources
    When Codex is installed through <manager>
    Then the private bundle includes those contracts and runtime resources unchanged
    And a consumer index links only installed contracts and method entry points
    And no maintainer specifications or producer-only contract documents are installed
    And the source checkout and consumer instructions are preserved

    Examples:
      | manager  |
      | Skillfile |
      | apm      |

  @B-007
  Scenario: Private runtime selection preserves non-specification nested resources
    Given a method with binary and Markdown runtime resources in nested directories
    And maintainer specifications at multiple depths
    When Codex is installed
    Then runtime resource paths and bytes are preserved
    And every specification directory is excluded

  @B-007 @B-006
  Scenario: A selected role's missing runtime dependency fails before effects
    Given a selected role refers to a missing private contract
    When Codex installation is planned
    Then installation fails before manager execution or consumer writes
