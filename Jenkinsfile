pipeline {
    agent any

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        CONFIGURATION = 'Release'
        ARTIFACTS_DIR = "${WORKSPACE}\\artifacts"
        SLN_PATH = "${WORKSPACE}\\student-management-new.sln"
        CSPROJ_PATH = "${WORKSPACE}\\student-management-new.csproj"
        DEPLOY_PATH = "C:\\inetpub\\wwwroot\\student-management-new"
        APP_POOL_NAME = "DefaultAppPool"
    }

    stages {
        stage('Checkout') {
            steps {
                echo '📥 Cloning source code from GitHub...'
                git 'https://github.com/duy1707gg/student-management-new.git'
            }
        }

        stage('Clean') {
            steps {
                echo '🧹 Cleaning artifacts folder...'
                bat "IF EXIST \"${ARTIFACTS_DIR}\" rmdir /S /Q \"${ARTIFACTS_DIR}\""
                bat "mkdir \"${ARTIFACTS_DIR}\""
            }
        }

        stage('Restore') {
            steps {
                echo '🔧 Restoring packages...'
                bat "dotnet restore \"${SLN_PATH}\""
            }
        }

        stage('Build') {
            steps {
                echo '⚙️ Building project...'
                bat "dotnet build \"${SLN_PATH}\" --configuration ${CONFIGURATION} --no-restore"
            }
        }

        stage('Publish') {
            steps {
                echo '📦 Publishing project...'
                bat "dotnet publish \"${CSPROJ_PATH}\" -c ${CONFIGURATION} -o \"${ARTIFACTS_DIR}\""
            }
        }

        stage('Stop IIS AppPool') {
            steps {
                echo '⛔ Stopping AppPool...'
                powershell '''
                    try {
                        Import-Module WebAdministration -ErrorAction Stop
                        if (Test-Path "IIS:\\AppPools\\${env:APP_POOL_NAME}") {
                            Stop-WebAppPool -Name "${env:APP_POOL_NAME}"
                            Write-Output "✔ App pool '${env:APP_POOL_NAME}' stopped."
                        } else {
                            Write-Warning "⚠ App pool '${env:APP_POOL_NAME}' does not exist."
                        }
                    } catch {
                        Write-Error "❌ Failed to stop app pool: $_"
                    }
                '''
            }
        }

        stage('Deploy to IIS') {
            steps {
                echo '🚀 Deploying to IIS...'
                bat "IF EXIST \"${DEPLOY_PATH}\" rmdir /S /Q \"${DEPLOY_PATH}\""
                bat "mkdir \"${DEPLOY_PATH}\""
                bat "robocopy \"${ARTIFACTS_DIR}\" \"${DEPLOY_PATH}\" /E /Z /NP /NFL /NDL /R:3 /W:5"
            }
        }

        stage('Start IIS AppPool') {
            steps {
                echo '▶️ Starting AppPool...'
                powershell '''
                    try {
                        Import-Module WebAdministration -ErrorAction Stop
                        if (Test-Path "IIS:\\AppPools\\${env:APP_POOL_NAME}") {
                            Start-WebAppPool -Name "${env:APP_POOL_NAME}"
                            Write-Output "✔ App pool '${env:APP_POOL_NAME}' started."
                        } else {
                            Write-Warning "⚠ App pool '${env:APP_POOL_NAME}' does not exist."
                        }
                    } catch {
                        Write-Error "❌ Failed to start app pool: $_"
                    }
                '''
            }
        }

        stage('List deployed files') {
            steps {
                echo '📂 Deployed files:'
                bat "dir \"${DEPLOY_PATH}\" /S"
            }
        }
    }

    post {
        failure {
            echo '❌ CI/CD failed. Check the logs for issues.'
        }
        success {
            echo '✅ CI/CD pipeline completed successfully.'
        }
    }
}
