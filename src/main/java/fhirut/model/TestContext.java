package fhirut.model;

import java.io.File;
import java.util.List;

public class TestContext {
    private List<String> igs;
    private List<String> profiles;
    private List<File> resources;

    // Getters e Setters

    public List<String> getIgs() {
        return igs;
    }

    public void setIgs(List<String> igs) {
        this.igs = igs;
    }

    public List<String> getProfiles() {
        return profiles;
    }

    public void setProfiles(List<String> profiles) {
        this.profiles = profiles;
    }

    public List<File> getResources() {
        return resources;
    }

    public void setResources(List<File> resources) {
        this.resources = resources;
    }
}
