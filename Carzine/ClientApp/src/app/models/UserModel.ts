export type UserModel = {
    email: string | undefined;
    isProfUser: boolean;
}

export type UserSessionModel = {
    access_token: string | null,
    access_token_expires: Date | null,
    userName: string | null,
    isProfUser: boolean
}