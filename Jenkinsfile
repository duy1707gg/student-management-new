pipeline {
    agent any

    environment {
        SOLUTION_NAME = 'student-management-new.sln'
        PROJECT_NAME = 'student-management-new.csproj'
        ARTIFACTS_DIR = "${WORKSPACE}\\artifacts"
        DEPLOY_DIR = 'C:\\inetpub\\wwwroot\\student-management-new'
        APP_POOL_NAME = 'DefaultAppPool'
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
                bat """
                    IF EXIST "${ARTIFACTS_DIR}" rmdir /S /Q "${ARTIFACTS_DIR}"
                    mkdir "${ARTIFACTS_DIR}"
                """
            }
        }

        stage('Restore') {
            steps {
                echo '🔧 Restoring packages...'
                bat "dotnet restore \"${SOLUTION_NAME}\""
            }
        }

        stage('Build') {
            steps {
                echo '⚙️ Building project...'
                bat "dotnet build \"${SOLUTION_NAME}\" --configuration Release --no-restore"
            }
        }

        stage('Publish') {
            steps {
                echo '📦 Publishing project...'
                bat "dotnet publish \"${PROJECT_NAME}\" -c Release -o \"${ARTIFACTS_DIR}\""
            }
        }

        stage('Stop IIS AppPool') {
            steps {
                echo '⛔ Stopping AppPool...'
                powershell '''
                    try {
                        Import-Module WebAdministration -ErrorAction Stop
                        if (Test-Path "IIS:\\AppPools\\$env:APP_POOL_NAME") {
                            Stop-WebAppPool -Name "$env:APP_POOL_NAME" -ErrorAction SilentlyContinue
                            Write-Output "✔ App pool '$env:APP_POOL_NAME' stopped."
                        } else {
                            Write-Warning "⚠ App pool '$env:APP_POOL_NAME' does not exist."
                        }
                    } catch {
                        Write-Warning "⚠ Error stopping AppPool: $_"
                    }
                    exit 0
                '''
            }
        }

        stage('Deploy to IIS') {
            steps {
                echo '🚀 Deploying to IIS...'
                bat """
                    IF EXIST "${DEPLOY_DIR}" rmdir /S /Q "${DEPLOY_DIR}"
                    mkdir "${DEPLOY_DIR}"
                    robocopy "${ARTIFACTS_DIR}" "${DEPLOY_DIR}" /E /Z /NP /NFL /NDL /R:3 /W:5
                """
            }
        }

stage('Start IIS AppPool') {
    steps {
        echo '▶ Starting AppPool...'
        powershell(returnStatus: true, script: '''
            try {
                Import-Module WebAdministration -ErrorAction Stop
                if (Test-Path "IIS:\\AppPools\\$env:APP_POOL_NAME") {
                    Start-WebAppPool -Name "$env:APP_POOL_NAME" -ErrorAction SilentlyContinue
                    Write-Output "✔ App pool '$env:APP_POOL_NAME' started."
                    exit 0
                } else {
                    Write-Warning "⚠ App pool '$env:APP_POOL_NAME' does not exist."
                    exit 0
                }
            } catch {
                Write-Warning "⚠ Error starting AppPool: $_"
                exit 0
            }
        ''')
    }
}


        stage('List deployed files') {
            steps {
                echo '📁 Deployed files:'
                bat "dir \"${DEPLOY_DIR}\""
            }
        }
    }

    post {
        success {
            echo '✅ CI/CD completed successfully!'
        }
        failure {
            echo '❌ CI/CD failed. Check the logs for issues.'
        }
    }
}
