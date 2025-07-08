pipeline {
    agent any

    environment {
        GIT_REPO = 'https://github.com/duy1707gg/student-management-new.git'
        SOLUTION = "${env.WORKSPACE}\\student-management-new.sln"
        CSPROJ = "${env.WORKSPACE}\\student-management-new.csproj"
        ARTIFACT_PATH = "${env.WORKSPACE}\\artifacts"
        IIS_DEPLOY_PATH = 'C:\\inetpub\\wwwroot\\student-management-new'
        APP_POOL_NAME = 'DefaultAppPool'
    }

    stages {
        stage('Checkout') {
            steps {
                echo '📥 Cloning source code from GitHub...'
                git branch: 'master', url: "${env.GIT_REPO}"
            }
        }

        stage('Clean') {
            steps {
                echo '🧹 Cleaning artifacts folder...'
                bat "IF EXIST \"${env.ARTIFACT_PATH}\" rmdir /S /Q \"${env.ARTIFACT_PATH}\""
                bat "mkdir \"${env.ARTIFACT_PATH}\""
            }
        }

        stage('Restore') {
            steps {
                echo '🔧 Restoring packages...'
                bat "dotnet restore \"${env.WORKSPACE}\\student-management-new.sln\""
            }
        }

        stage('Build') {
            steps {
                echo '⚙️ Building project...'
                bat "dotnet build \"${env.SOLUTION}\" --configuration Release --no-restore"
            }
        }

        stage('Publish') {
            steps {
                echo '📦 Publishing project...'
                bat "dotnet publish \"${env.CSPROJ}\" -c Release -o \"${env.ARTIFACT_PATH}\""
            }
        }

        stage('Stop IIS AppPool') {
            steps {
                echo '⛔ Stopping AppPool...'
                powershell '''
                    Import-Module WebAdministration
                    if (Test-Path "IIS:\\AppPools\\${env:APP_POOL_NAME}") {
                        Stop-WebAppPool -Name "${env:APP_POOL_NAME}"
                    }
                '''
            }
        }

        stage('Deploy to IIS') {
            steps {
                echo '🚀 Deploying to IIS...'
                bat """
                    IF EXIST \"${env.IIS_DEPLOY_PATH}\" rmdir /S /Q \"${env.IIS_DEPLOY_PATH}\"
                    mkdir \"${env.IIS_DEPLOY_PATH}\"
                    robocopy \"${env.ARTIFACT_PATH}\" \"${env.IIS_DEPLOY_PATH}\" /E /Z /NP /NFL /NDL /R:3 /W:5
                """
            }
        }

        stage('Start IIS AppPool') {
            steps {
                echo '▶️ Starting AppPool...'
                powershell '''
                    Import-Module WebAdministration
                    if (Test-Path "IIS:\\AppPools\\${env:APP_POOL_NAME}") {
                        Start-WebAppPool -Name "${env:APP_POOL_NAME}"
                    }
                '''
            }
        }

        stage('List deployed files') {
            steps {
                echo '📂 Listing files...'
                bat "dir \"${env.IIS_DEPLOY_PATH}\" /s"
            }
        }
    }

    post {
        success {
            echo "✅ CI/CD pipeline completed successfully!"
        }
        failure {
            echo "❌ CI/CD failed. Check the logs for issues."
        }
    }
}
